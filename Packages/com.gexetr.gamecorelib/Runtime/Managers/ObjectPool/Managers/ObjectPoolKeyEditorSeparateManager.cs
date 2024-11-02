using System.Collections.Generic;
using Game.Factories;
using UnityEngine;

namespace Game.Pools.Managers
{
internal class ObjectPoolKeyEditorSeparateManager : ObjectPoolKeyManager
{
    private readonly Dictionary<string, Transform> _pool;

    public ObjectPoolKeyEditorSeparateManager(IFactoryGameObjects objectFactoryGameObjects,
                                              Transform poolRoot,
                                              int capacity)
        : base(objectFactoryGameObjects, poolRoot, capacity)
    {
        _pool = new Dictionary<string, Transform>(defaultCapacity);
    }

    protected override void Warn<T>(T prefab, int expectedCountNewElements)
    {
        base.Warn(prefab, expectedCountNewElements);

        if (_pool.ContainsKey(prefab.Key))
            return;
        var root = base.GetPoolRoot(prefab);
        var parent = prefab.IsUiElement ? CreateAndSetupUIObjectPoolRoot(root) : CreateObjectPoolRoot(root);
        if (Application.isEditor)
            parent.name = $"[{_pool.Count}] {prefab.Key}";
        _pool.Add(prefab.Key, parent);
    }

    protected override Transform GetPoolRoot(IPoolable poolableObject) => _pool[poolableObject.Key];
}
}