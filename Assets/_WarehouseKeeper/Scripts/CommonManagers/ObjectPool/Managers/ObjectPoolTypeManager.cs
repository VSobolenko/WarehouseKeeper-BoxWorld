using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;
using WarehouseKeeper.Extension;
using WarehouseKeeper.Factories;

namespace WarehouseKeeper.Pools.Managers
{
internal class ObjectPoolTypeManager : BaseObjectPoolManager, IObjectPoolManager
{
    //<TPoolType>  where TPoolType : IObjectPool<IPoolable> ????
    private readonly Dictionary<System.Type, ObjectPool<IPoolable>> _keyPool;
    private int _defaultCapacity;

    public ObjectPoolTypeManager(IFactoryGameObjects objectFactoryGameObjects, Transform poolRoot, int capacity) 
        : base(objectFactoryGameObjects, poolRoot)
    {
        _defaultCapacity = Mathf.Max(0, capacity);
        _keyPool = new Dictionary<System.Type, ObjectPool<IPoolable>>(_defaultCapacity);
    }
    
    public void Prepare<T>(T prefab, int count) where T : Object, IPoolable
    {
        AddNewPoolElementIfNotExist(prefab);
        for (var i = 0; i < count; i++)
        {
            var item = CreateNewElement(prefab, GetPoolRoot(prefab));
            item.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            _keyPool[typeof(T)].Release(item);
        }
    }

    public async Task PrepareAsync<T>(T prefab, int count, CancellationToken token = default) where T : Object, IPoolable
    {
        AddNewPoolElementIfNotExist(prefab);
        
        for (var i = 0; i < count; i++)
        {
            if (token.IsCancellationRequested)
                return;
            var pooledObject = CreateNewElement(prefab, GetPoolRoot(prefab));
            pooledObject.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            _keyPool[typeof(T)].Release(pooledObject);
            
            await Cysharp.Threading.Tasks.UniTask.DelayFrame(1, cancellationToken: token);
        }
    }

    public T Get<T>(T prefab) where T : Object, IPoolable
    {
        AddNewPoolElementIfNotExist(prefab);
        var pooledObject = (T) _keyPool[typeof(T)].Get();
        
        pooledObject.SetParent(null);
        pooledObject.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        return pooledObject;
    }

    public T Get<T>(T prefab, Vector3 position, Quaternion rotation) where T : Object, IPoolable
    {
        AddNewPoolElementIfNotExist(prefab);
        var pooledObject = (T) _keyPool[typeof(T)].Get();
        
        pooledObject.SetParent(null);
        pooledObject.SetPositionAndRotation(position, rotation);

        return pooledObject;
    }

    public T Get<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent) where T : Object, IPoolable
    {
        AddNewPoolElementIfNotExist(prefab);
        var pooledObject = (T) _keyPool[typeof(T)].Get();
        
        pooledObject.SetParent(parent);
        pooledObject.SetPositionAndRotation(position, rotation);

        return pooledObject;
    }

    public T Get<T>(T prefab, Transform parent) where T : Object, IPoolable
    {
        AddNewPoolElementIfNotExist(prefab);
        var pooledObject = (T) _keyPool[typeof(T)].Get();
        
        pooledObject.SetParent(parent);
        pooledObject.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        return pooledObject;
    }
    
    public void Release<T>(T prefab) where T : Object, IPoolable
    {
        AddNewPoolElementIfNotExist(prefab);

        prefab.SetParent(GetPoolRoot(prefab));
        prefab.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        
        _keyPool[typeof(T)].Release(prefab);
    }

    private void AddNewPoolElementIfNotExist<T>(T prefab) where T : Object, IPoolable
    {
        if (_keyPool.ContainsKey(typeof(T)))
            return;

        var pool = new ObjectPool<IPoolable>(() => CreateNewElement(prefab, GetPoolRoot(prefab)),
                                                    OnGetFromPool, OnReturnedToPool, poolable => Log.InternalError());

        if (_keyPool.Count == _defaultCapacity)
            Log.WriteWarning("Pool capacity exceeded. Use an increased size of the original container");
        
        _keyPool.Add(typeof(T), pool);
        _defaultCapacity = _keyPool.Count;
    }

    private IPoolable CreateNewElement<T>(T prefab, Transform parent) where T : Object, IPoolable
    {
        var pooledObject = factoryGameObjects.Instantiate(prefab, parent);
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
}
}