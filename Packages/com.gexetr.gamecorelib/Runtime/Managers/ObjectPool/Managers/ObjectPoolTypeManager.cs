using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Game.Factories;
using Game.InternalData;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game.Pools.Managers
{
/// <summary>
/// Object pool by object type
/// If the object type is already present, other prefabs but with the same type are ignored
/// </summary>
internal class ObjectPoolTypeManager : IObjectPoolManager
{
    private readonly IFactoryGameObjects _factoryGameObjects;
    private readonly Dictionary<Type, IObjectPool<IPoolable>> _pool;
    private readonly Transform _root;
    private readonly Transform _rootUi;
    private readonly ObjectPoolProfilerProvider _poolProfiler;
    protected int defaultCapacity;

    public ObjectPoolTypeManager(IFactoryGameObjects objectFactoryGameObjects, Transform poolRoot, int capacity)
    {
        _factoryGameObjects = objectFactoryGameObjects;
        defaultCapacity = Mathf.Max(0, capacity);
        _pool = new Dictionary<Type, IObjectPool<IPoolable>>(defaultCapacity);

        _root = CreateObjectPoolRoot(poolRoot);
        _rootUi = CreateAndSetupUIObjectPoolRoot(poolRoot);
        _poolProfiler = SetupEditorHierarchyStructureAndCreatePoolProfiler(poolRoot);
    }

    protected Transform CreateObjectPoolRoot(Transform poolRoot) =>
        _factoryGameObjects.InstantiateEmpty(poolRoot).transform;

    protected Transform CreateAndSetupUIObjectPoolRoot(Transform poolRoot)
    {
        var rootUI = _factoryGameObjects
                     .InstantiateEmpty(poolRoot, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler),
                                       typeof(GraphicRaycaster)).transform;

        var canvas = rootUI.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.vertexColorAlwaysGammaSpace = true;

        return rootUI;
    }

    private ObjectPoolProfilerProvider SetupEditorHierarchyStructureAndCreatePoolProfiler(Transform poolRoot)
    {
        if (Application.isEditor == false)
            return null;

        poolRoot.name = $"{GameData.Identifier}.Pool";
        _root.name = $"{GameData.Identifier}.Root";
        _rootUi.name = $"{GameData.Identifier}.RootUI";

        if (poolRoot.TryGetComponent<ObjectPoolProfiler>(out var profilerView))
            return new ObjectPoolProfilerProvider(poolRoot)
                   .AssignProfilerView(profilerView)
                   .Initialize(this, _pool);

        return new ObjectPoolProfilerProvider(poolRoot).Initialize(this, _pool);
    }

    public void Prepare<T>(T prefab, int count) where T : Object, IPoolable
    {
        if (prefab == null)
            throw new ArgumentException($"Can't prepare null prefab");

        Warn(prefab, count);
        for (var i = 0; i < count; i++)
        {
            var parent = GetPoolRoot(prefab);
            var item = CreateNewElement(prefab, parent);
            Release((T) item);
        }
    }

    public async Task PrepareAsync<T>(T prefab, int count, CancellationToken token = default)
        where T : Object, IPoolable
    {
        Warn(prefab, count);

        for (var i = 0; i < count; i++)
        {
            if (token.IsCancellationRequested)
                return;

            Prepare(prefab, 1);

            if (token.IsCancellationRequested)
                return;
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

    public void Release<T>(T prefabInstance) where T : Object, IPoolable
    {
        if (prefabInstance == null)
            throw new ArgumentException($"Can't release null prefab");

        if (_pool.ContainsKey(prefabInstance.GetType()) == false)
            throw new ArgumentException(
                $"Return unknown prefab to pool. Use {nameof(Prepare)} first. PrefabType={prefabInstance.GetType()}");

        var parent = GetPoolRoot(prefabInstance);
        prefabInstance.SetParent(parent);
        prefabInstance.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

        _pool[prefabInstance.GetType()].Release(prefabInstance);
        _poolProfiler?.Update();
    }

    private T InternalGet<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent)
        where T : Object, IPoolable
    {
        if (prefab == null)
            throw new ArgumentException($"Can't prepare null prefab");

        if (_pool.ContainsKey(prefab.GetType()) == false)
            throw new ArgumentException($"An unknown object was requested. Use {nameof(Prepare)} first");

        var pooledObject = (T) _pool[prefab.GetType()].Get();

        pooledObject.SetParent(parent);
        pooledObject.SetPositionAndRotation(position, rotation);
        _poolProfiler?.Update();

        return pooledObject;
    }

    protected virtual void Warn<T>(T prefab, int expectedCountNewElements) where T : Object, IPoolable
    {
        if (_pool.ContainsKey(prefab.GetType()))
            return;

        var pool = new ObjectPool<IPoolable>(() => CreateNewElement(prefab, GetPoolRoot(prefab)),
                                             OnGetFromPool, OnReturnedToPool, poolable => Log.InternalError(), true,
                                             expectedCountNewElements);

        if (_pool.Count > defaultCapacity)
        {
            Log.Warning("Pool capacity exceeded. Use an increased size of the original container");
            defaultCapacity = _pool.Count;
        }

        _pool.Add(prefab.GetType(), pool);
        _poolProfiler?.Update();
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

    protected virtual Transform GetPoolRoot(IPoolable poolableObject) => poolableObject.IsUiElement ? _rootUi : _root;
}
}