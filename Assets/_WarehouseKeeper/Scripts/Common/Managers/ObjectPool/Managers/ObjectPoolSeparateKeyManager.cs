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
/// Same as "ObjectPoolKeyManager"
/// Added division in the hierarchy for easy testing
/// </summary>
internal class ObjectPoolSeparateKeyManager : IObjectPoolManager
{
    private readonly IFactoryGameObjects _factoryGameObjects;
    private readonly Dictionary<string, PoolableData> _keyPool;
    private readonly Transform _root;
    private int _defaultCapacity;

    public ObjectPoolSeparateKeyManager(IFactoryGameObjects objectFactoryGameObjects, Transform poolRoot, int capacity)
    {
        _factoryGameObjects = objectFactoryGameObjects;

        _root = poolRoot;
        _defaultCapacity = Mathf.Max(0, capacity);
        _keyPool = new Dictionary<string, PoolableData>(_defaultCapacity);

#if UNITY_EDITOR
        if (poolRoot != null)
            poolRoot.name = $"{Application.productName}.Pool";
        
        var poolProfiler = poolRoot.gameObject.AddComponent<ObjectPoolProfilerProvider>();
        poolProfiler.AssignPool(this, _keyPool);
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
        
        if (_keyPool.ContainsKey(prefab.Key) == false || _keyPool[prefab.Key].pools.Count == 0)
            AddElementsToPool(prefab, false);
        
        var pooledObject = (T) _keyPool[prefab.Key].pools.Pop();
        pooledObject.SetParent(parent);
        pooledObject.SetPositionAndRotation(position, rotation);
        pooledObject.SetActive(true);
        pooledObject.OnUse();
        
        return pooledObject;
    }
    
    private void AddElementsToPool<T>(T prefab, bool isInstance, int expectedCountNewElements = 1) where T : Object, IPoolable
    {
        if (prefab == null)
        {
            Log.WriteWarning("Can't add null prefab to pool");
            return;
        }

        if (_keyPool.ContainsKey(prefab.Key) == false)
        {
            if (_keyPool.Keys.Count > _defaultCapacity)
            {
                Log.WriteWarning("Pool capacity exceeded. Use an increased size of the original container");
                _defaultCapacity = _keyPool.Count;
            }

            _keyPool.Add(prefab.Key, new PoolableData
            {
                pools = new Stack<IPoolable>(expectedCountNewElements),
                root = GetNewPoolRoot(prefab, _keyPool.Count - 1), 
            });
        }

        var pooledObject = isInstance ? prefab : _factoryGameObjects.Instantiate(prefab, _keyPool[prefab.Key].root);

        _keyPool[prefab.Key].pools.Push(pooledObject);
        if (isInstance)
        {
            pooledObject.SetParent(_keyPool[prefab.Key].root);
            pooledObject.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
        pooledObject.SetActive(false);
        pooledObject.OnRelease();
        pooledObject.Pool = this;
    }
    
    private Transform GetNewPoolRoot(IPoolable poolableObject, int index)
    {
        Transform root;
        if (poolableObject.IsUiElement)
        {
            root = _factoryGameObjects.InstantiateEmpty(_root, typeof(RectTransform), typeof(Canvas),
                                                        typeof(CanvasScaler), typeof(GraphicRaycaster)).transform;

            root.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        }
        else
        {
            root = _factoryGameObjects.InstantiateEmpty(_root).transform;
        }

#if UNITY_EDITOR
        root.name = $"[{index}]{poolableObject.Key}";
#endif

        return root;
    }
    
    internal struct PoolableData
    {
        public Transform root;
        public Stack<IPoolable> pools;
    }
}
}