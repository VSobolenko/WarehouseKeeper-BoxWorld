using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using WarehouseKeeper.Extension;
using WarehouseKeeper.Factories;

namespace WarehouseKeeper.Pools.Implementation
{
/// <summary>
/// Object pool by key type
/// Identical objects with the same type are represented as different if the keys are different
/// </summary>
internal class ObjectPoolKeyManager : IObjectPoolManager
{
    private readonly IFactoryGameObjects _factoryGameObjects;
    private readonly Dictionary<string, Stack<IPoolable>> _keyPool;
    private readonly Transform _root;
    private readonly Transform _rootUi;
    private int _defaultCapacity;

#if UNITY_EDITOR
    private readonly ObjectPoolProfilerProvider _poolProfiler;
#endif
    
    public ObjectPoolKeyManager(IFactoryGameObjects objectFactoryGameObjects, Transform poolRoot, int capacity)
    {
        _factoryGameObjects = objectFactoryGameObjects;

        _root = objectFactoryGameObjects.InstantiateEmpty(poolRoot).transform;

        _rootUi = objectFactoryGameObjects
                  .InstantiateEmpty(poolRoot, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler),
                                    typeof(GraphicRaycaster)).transform;

        _rootUi.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        
        _defaultCapacity = Mathf.Max(0, capacity);
        _keyPool = new Dictionary<string, Stack<IPoolable>>(_defaultCapacity);

#if UNITY_EDITOR
        if (poolRoot != null)
            poolRoot.name = $"{Application.productName}.Pool";

        if (poolRoot.GetComponent<ObjectPoolProfilerProvider>() == null)
        {
            _poolProfiler = poolRoot.gameObject.AddComponent<ObjectPoolProfilerProvider>();
            _poolProfiler.AssignPool(this, _keyPool);   
        }
        
        _root.name = $"{Application.productName}.Root";
        _rootUi.name = $"{Application.productName}.RootUI";
#endif
    }

    public void Prepare<T>(T prefab, int count) where T : Object, IPoolable
    {
        if (prefab == null)
        {
            Log.WriteWarning("Can't prepare null prefab");
            return;
        }
        
        for (var i = 0; i < count; i++)
            AddElementsToPool(prefab, false, count);
    }

    public async Task PrepareAsync<T>(T prefab, int count, CancellationToken token = default) where T : Object, IPoolable
    {
        if (prefab == null)
        {
            Log.WriteWarning("Can't prepare null prefab");
            return;
        }
        
        for (var i = 0; i < count; i++)
        {
            if (token.IsCancellationRequested)
                return;
            
            AddElementsToPool(prefab, false, count);
            await UniTask.DelayFrame(1, cancellationToken: token);
        }
    }

    public T Get<T>(T prefab) where T : Object, IPoolable =>
        InternalGet(prefab, Vector3.zero, Quaternion.identity, null);

    public T Get<T>(T prefab, Vector3 position, Quaternion rotation) where T : Object, IPoolable =>
        InternalGet(prefab, position, rotation, null);

    public T Get<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent) where T : Object, IPoolable =>
        InternalGet(prefab, position, rotation, parent);

    public T Get<T>(T prefab, Transform parent) where T : Object, IPoolable =>
        InternalGet(prefab, Vector3.zero, Quaternion.identity, parent);

    public void Release<T>(T prefab) where T : Object, IPoolable
    {
        if (prefab != null && _keyPool.ContainsKey(prefab.Key) == false)
            Log.WriteError("Return unknown prefab to pool. Pool capacity increase");
        
        AddElementsToPool(prefab, true);
    }

    private T InternalGet<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent)
        where T : Object, IPoolable
    {
        if (prefab == null)
        {
            Log.WriteWarning("Can't get null prefab");
            return null;
        }
        
        if (_keyPool.ContainsKey(prefab.Key) == false || _keyPool[prefab.Key].Count == 0)
            AddElementsToPool(prefab, false);
        
        var pooledObject = (T) _keyPool[prefab.Key].Pop();
        pooledObject.SetParent(parent);
        pooledObject.SetPositionAndRotation(position, rotation);
        pooledObject.SetActive(true);
        pooledObject.OnUse();
        
#if UNITY_EDITOR
        _poolProfiler.UpdateProfiler();
#endif
        return pooledObject;
    }
    
    private void AddElementsToPool<T>(T prefab, bool isInstance, int expectedCountNewElements = 1) where T : Object, IPoolable
    {
        if (prefab == null)
        {
            Log.WriteWarning("Can't add null prefab to pool");
            return;
        }
        var pooledRoot = GetPoolRoot(prefab);
        var pooledObject = isInstance ? prefab : _factoryGameObjects.Instantiate(prefab, pooledRoot);

        if (_keyPool.ContainsKey(prefab.Key) == false)
        {
            if (_keyPool.Keys.Count > _defaultCapacity)
            {
                Log.WriteWarning("Pool capacity exceeded. Use an increased size of the original container");
                _defaultCapacity = _keyPool.Count;
            }

            _keyPool.Add(prefab.Key, new Stack<IPoolable>(expectedCountNewElements));
        }

        _keyPool[prefab.Key].Push(pooledObject);
        if (isInstance)
        {
            pooledObject.SetParent(pooledRoot);
            pooledObject.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
        pooledObject.SetActive(false);
        pooledObject.OnRelease();
        pooledObject.Pool = this;
        
#if UNITY_EDITOR
        _poolProfiler.UpdateProfiler();
#endif
    }
    
    private Transform GetPoolRoot(IPoolable poolableObject) => poolableObject.IsUiElement ? _rootUi : _root;
}
}