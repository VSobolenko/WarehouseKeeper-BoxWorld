using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using WarehouseKeeper.Extension;
using WarehouseKeeper.Factories;

namespace WarehouseKeeper.Pools.Implementation
{
/// <summary>
/// Object pool by object type
/// If the object type is already present, other prefabs but with the same type are ignored
/// </summary>
internal class ObjectPoolTypeManager : IObjectPoolManager
{
    private readonly IFactoryGameObjects _factoryGameObjects;
    private readonly Dictionary<System.Type, ObjectPool<IPoolable>> _typePool;
    private readonly Transform _root;
    private readonly Transform _rootUi;
    private int _defaultCapacity;

    public ObjectPoolTypeManager(IFactoryGameObjects objectFactoryGameObjects, Transform poolRoot, int capacity) 
    {
        _factoryGameObjects = objectFactoryGameObjects;

        _root = objectFactoryGameObjects.InstantiateEmpty(poolRoot).transform;

        _rootUi = objectFactoryGameObjects
                  .InstantiateEmpty(poolRoot, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler),
                                    typeof(GraphicRaycaster)).transform;

        _rootUi.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        
        _defaultCapacity = Mathf.Max(0, capacity);
        _typePool = new Dictionary<System.Type, ObjectPool<IPoolable>>(_defaultCapacity);

#if UNITY_EDITOR
        if (poolRoot != null)
            poolRoot.name = $"{Application.productName}.Pool";

        _root.name = $"{Application.productName}.Root";
        _rootUi.name = $"{Application.productName}.RootUI";
        
        var poolProfiler = poolRoot.gameObject.AddComponent<ObjectPoolProfilerProvider>();
        poolProfiler.AssignPool(this, _typePool);
#endif
    }
    
    public void Prepare<T>(T prefab, int count) where T : Object, IPoolable
    {
        if (prefab == null)
        {
            Log.WriteWarning("Can't prepare null prefab");
            return;
        }
        
        AddNewPoolElementIfNotExist(prefab);
        for (var i = 0; i < count; i++)
        {
            var item = CreateNewElement(prefab, GetPoolRoot(prefab));
            item.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            _typePool[typeof(T)].Release(item);
        }
    }

    public async Task PrepareAsync<T>(T prefab, int count, CancellationToken token = default) where T : Object, IPoolable
    {
        if (prefab == null)
        {
            Log.WriteWarning("Can't prepare null prefab");
            return;
        }
        
        AddNewPoolElementIfNotExist(prefab);
        
        for (var i = 0; i < count; i++)
        {
            if (token.IsCancellationRequested)
                return;
            var pooledObject = CreateNewElement(prefab, GetPoolRoot(prefab));
            pooledObject.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            _typePool[typeof(T)].Release(pooledObject);
            
            await Cysharp.Threading.Tasks.UniTask.DelayFrame(1, cancellationToken: token);
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
        if (prefab != null && _typePool.ContainsKey(prefab.GetType()) == false)
            Log.WriteError("Return unknown prefab to pool. Pool capacity increase");
        
        AddNewPoolElementIfNotExist(prefab);

        prefab.SetParent(GetPoolRoot(prefab));
        prefab.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        
        _typePool[typeof(T)].Release(prefab);
    }

    private T InternalGet<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent)
        where T : Object, IPoolable
    {
        if (prefab == null)
        {
            Log.WriteWarning("Can't get null prefab");
            return null;
        }
        
        AddNewPoolElementIfNotExist(prefab);
        var pooledObject = (T) _typePool[typeof(T)].Get();
        
        pooledObject.SetParent(parent);
        pooledObject.SetPositionAndRotation(position, rotation);

        return pooledObject;
    }
    
    private void AddNewPoolElementIfNotExist<T>(T prefab) where T : Object, IPoolable
    {
        if (_typePool.ContainsKey(typeof(T)))
            return;

        var pool = new ObjectPool<IPoolable>(() => CreateNewElement(prefab, GetPoolRoot(prefab)),
                                                    OnGetFromPool, OnReturnedToPool, poolable => Log.InternalError());

        if (_typePool.Count == _defaultCapacity)
            Log.WriteWarning("Pool capacity exceeded. Use an increased size of the original container");
        
        _typePool.Add(typeof(T), pool);
        _defaultCapacity = _typePool.Count;
    }

    private IPoolable CreateNewElement<T>(T prefab, Transform parent) where T : Object, IPoolable
    {
        var pooledObject = _factoryGameObjects.Instantiate(prefab, parent);
        pooledObject.SetActive(false);
        pooledObject.Pool = this;
        
        return pooledObject;
    }

    private static void OnGetFromPool<T>(T pooledObject) where T : IPoolable
    {
        pooledObject.SetActive(true);
        pooledObject.OnUse();
    }

    private static void OnReturnedToPool<T>(T pooledObject) where T : IPoolable
    {
        pooledObject.SetActive(false);
        pooledObject.OnRelease();
    }
    
    private Transform GetPoolRoot(IPoolable poolableObject) => poolableObject.IsUiElement ? _rootUi : _root;
}
}