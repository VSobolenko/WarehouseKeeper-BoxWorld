using System.Collections.Generic;
using Game.Pools;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameEditor.Pools
{
internal class SeparateKeyManagerProfiler : IPoolProfiler
{
    private readonly Dictionary<string, Stack<IPoolable>> _pool;

    public SeparateKeyManagerProfiler(object pool)
    {
        _pool = pool as Dictionary<string, Stack<IPoolable>>;
        if (_pool == null)
            Debug.LogError($"Can't unboxing pool dictionary for {GetType().Name} profiler");
    }

    public void DrawStatus(VisualElement root)
    {
        if (_pool == null)
            return;
        
        GUILayout.Label($"Profiler type: {GetType().Name}");
        GUILayout.Label($"Pool capacity: {_pool.Keys.Count}");
    }
    
    public void OnPoolDataUpdated() { }
}
}