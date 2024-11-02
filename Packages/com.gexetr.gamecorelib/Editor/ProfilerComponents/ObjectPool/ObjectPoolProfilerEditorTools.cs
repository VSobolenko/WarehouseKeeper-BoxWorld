using System;
using System.Collections.Generic;
using Game.Pools;
using Game.Pools.Managers;
using UnityEditor;
using UnityEngine;

namespace GameEditor.Pools
{
/// <summary>
/// Profiler for object pool
/// Always show on the root of pool in hierarchy
/// Display necessary information about pool state, provided by a specific profiler for a specific manager
/// _poolProfilerTypes - establishes a dependency between profiler and manager
/// </summary>
[CustomEditor(typeof(ObjectPoolProfiler))]
public class ObjectPoolProfilerEditorTools : Editor
{
    private IPoolProfiler _poolProfiler;

    /// <summary>
    /// Key - manager
    /// Value - profiler
    /// </summary>
    private readonly Dictionary<Type, Type> _poolProfilerTypes = new()
    {
        {typeof(ObjectPoolKeyManager), typeof(KeyManagerProfiler)},
        {typeof(ObjectPoolKeyEditorSeparateManager), typeof(KeyManagerProfiler)},
        {typeof(ObjectPoolTypeManager), typeof(TypeManagerProfiler)},
        {typeof(ObjectPoolTypeEditorSeparateManager), typeof(TypeManagerProfiler)},
    };
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var root = CreateInspectorGUI();
        _poolProfiler?.DrawStatus(root);
    }
    
    private void OnEnable()
    {
        var provider = target as ObjectPoolProfiler;
        if (provider == null)
            return;
        SetupPoolProfiler(provider);
        provider.OnPoolUpdate += SetupPoolProfiler;
        if (_poolProfiler != null)
            provider.OnPoolStructureUpdate += _poolProfiler.OnPoolDataUpdated;
        provider.OnPoolStructureUpdate += Repaint;
    }

    private void OnDisable()
    {
        var provider = target as ObjectPoolProfiler;
        if (provider == null)
            return;
        provider.OnPoolUpdate -= SetupPoolProfiler;
        if (_poolProfiler != null)
            provider.OnPoolStructureUpdate -= _poolProfiler.OnPoolDataUpdated;
        provider.OnPoolStructureUpdate -= Repaint;
    }

    private void SetupPoolProfiler(ObjectPoolProfiler poolProfiler)
    {
        if (poolProfiler == null || poolProfiler.PoolManager == null || poolProfiler.PoolContainer == null)
        {
            Debug.LogError("Trying assign null pool entity");
            return;
        }
        
        var managerType = poolProfiler.PoolManager.GetType();
        if (_poolProfilerTypes.TryGetValue(managerType, out var profilerType) == false)
        {
            Debug.LogError($"For {managerType.Name} type profiler not found");
            return;
        }
        
        _poolProfiler = Activator.CreateInstance(profilerType, poolProfiler.PoolContainer) as IPoolProfiler;
    }
}
}