using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using WarehouseKeeper.Extension;
using WarehouseKeeper.Factories;

namespace WarehouseKeeper.Pools.Managers
{
internal class ObjectPoolKeyManager : BaseObjectPoolManager, IObjectPoolManager
{
    private readonly Dictionary<string, Stack<IPoolable>> _keyPool;
    private int _defaultCapacity;

    public ObjectPoolKeyManager(IFactoryGameObjects objectFactoryGameObjects, Transform poolRoot, int capacity) 
        : base(objectFactoryGameObjects, poolRoot)
    {
        _defaultCapacity = Mathf.Max(0, capacity);
        _keyPool = new Dictionary<string, Stack<IPoolable>>(_defaultCapacity);
    }

    public void Prepare<T>(T prefab, int count) where T : Object, IPoolable
    {
        for (var i = 0; i < count; i++)
        {
            AddElementsToPool(prefab, false, count);
        }
    }

    public async Task PrepareAsync<T>(T prefab, int count, CancellationToken token = default) where T : Object, IPoolable
    {
        for (var i = 0; i < count; i++)
        {
            if (token.IsCancellationRequested)
                return;
            
            AddElementsToPool(prefab, false, count);
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

    public void Release<T>(T prefab) where T : Object, IPoolable => AddElementsToPool(prefab, true);

    private T InternalGet<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent)
        where T : Object, IPoolable
    {
        if (_keyPool.ContainsKey(prefab.Key) == false || _keyPool[prefab.Key].Count == 0)
            AddElementsToPool(prefab, false);
        
        var pooledObject = (T) _keyPool[prefab.Key].Pop();
        pooledObject.SetParent(parent);
        pooledObject.SetPositionAndRotation(position, rotation);
        pooledObject.SetActive(true);
        pooledObject.OnUse();
        
        return pooledObject;
    }
    
    private void AddElementsToPool<T>(T prefab, bool isInstance, int expectedCountNewElements = 1) where T : Object, IPoolable
    {
        var pooledRoot = GetPoolRoot(prefab);
        var pooledObject = isInstance ? prefab : factoryGameObjects.Instantiate(prefab, pooledRoot);

        if (_keyPool.ContainsKey(prefab.Key) == false)
        {
            if (_keyPool.Count == _defaultCapacity)
                Log.WriteWarning("Pool capacity exceeded. Use an increased size of the original container");

            _keyPool.Add(prefab.Key, new Stack<IPoolable>(expectedCountNewElements));
            _defaultCapacity = _keyPool.Count;
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
    }
}
}