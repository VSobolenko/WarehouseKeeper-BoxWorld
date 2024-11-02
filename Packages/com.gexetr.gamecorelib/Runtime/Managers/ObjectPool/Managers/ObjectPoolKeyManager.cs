using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Game.Factories;
using Game.InternalData;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game.Pools.Managers
{
/// <summary>
/// Object pool by key type
/// Identical objects with the same type are represented as different if the keys are different
/// </summary>
internal class ObjectPoolKeyManager : IObjectPoolManager
{
    private readonly Dictionary<string, Stack<IPoolable>> _pool;
    private readonly Transform _root;
    private readonly Transform _rootUi;
    private readonly ObjectPoolProfilerProvider _poolProfiler;
    private readonly IFactoryGameObjects _factoryGameObjects;
    protected int defaultCapacity;

    public ObjectPoolKeyManager(IFactoryGameObjects objectFactoryGameObjects, Transform poolRoot, int capacity)
    {
        _factoryGameObjects = objectFactoryGameObjects;
        defaultCapacity = Mathf.Max(0, capacity);
        _pool = new Dictionary<string, Stack<IPoolable>>(defaultCapacity);

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
            CreateOrReturnElementToPool(prefab, false);
        _poolProfiler?.Update();
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
            _poolProfiler?.Update();

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

        if (_pool.ContainsKey(prefabInstance.Key) == false)
            throw new ArgumentException(
                $"Return unknown prefab to pool. Use {nameof(Prepare)} first. PrefabKey={prefabInstance.Key}");

        CreateOrReturnElementToPool(prefabInstance, true);
    }

    private T InternalGet<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent)
        where T : Object, IPoolable
    {
        if (prefab == null)
            throw new ArgumentException($"Can't get null prefab");

        if (_pool.ContainsKey(prefab.Key) == false)
            throw new ArgumentException($"An unknown object was requested. Use {nameof(Prepare)} first");

        if (_pool[prefab.Key].Count == 0)
            CreateOrReturnElementToPool(prefab, false);

        var pooledObject = _pool[prefab.Key].Pop();
        pooledObject.SetParent(parent);
        pooledObject.SetPositionAndRotation(position, rotation);
        pooledObject.SetActive(true);
        pooledObject.OnUse();

        _poolProfiler?.Update();

        return (T) pooledObject;
    }

    private void CreateOrReturnElementToPool<T>(T prefab, bool isInstance)
        where T : Object, IPoolable
    {
        var parent = GetPoolRoot(prefab);
        var pooledObject = isInstance ? prefab : _factoryGameObjects.Instantiate(prefab, parent);

        _pool[prefab.Key].Push(pooledObject);
        if (isInstance)
        {
            pooledObject.SetParent(parent);
            pooledObject.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        pooledObject.SetActive(false);
        pooledObject.OnRelease();
        pooledObject.Pool = this;

        _poolProfiler?.Update();
    }

    protected virtual void Warn<T>(T prefab, int expectedCountNewElements) where T : Object, IPoolable
    {
        if (_pool.ContainsKey(prefab.Key))
            return;

        if (_pool.Keys.Count > defaultCapacity)
        {
            Log.Warning("Pool capacity exceeded. Use an increased size of the original container");
            defaultCapacity = _pool.Count;
        }

        _pool.Add(prefab.Key, new Stack<IPoolable>(expectedCountNewElements));
    }

    protected virtual Transform GetPoolRoot(IPoolable poolableObject) => poolableObject.IsUiElement ? _rootUi : _root;
}
}