using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Game.Factories;
using Game.InternalData;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pools.Managers
{
/// <summary>
/// Object pool by object type
/// Same as "ObjectPoolTypeManager"
/// Added division in the hierarchy for easy testing
/// </summary>
internal class ObjectPoolTypeEditorSeparateManager : ObjectPoolTypeManager
{
    private readonly Dictionary<string, Transform> _pool;

    public ObjectPoolTypeEditorSeparateManager(IFactoryGameObjects objectFactoryGameObjects, Transform poolRoot, int capacity)
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