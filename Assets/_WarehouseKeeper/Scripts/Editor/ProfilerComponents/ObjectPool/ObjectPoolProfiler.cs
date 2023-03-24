using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using WarehouseKeeper.Pools.Implementation;

namespace WarehouseKeeper.EditorScripts.ObjectPool
{
/// <summary>
/// Profiler for object pool
/// Always show on the root of pool in hierarchy
/// Display necessary information about pool state, provided by a specific profiler for a specific manager
/// _poolProfilerTypes - establishes a dependency between profiler and manager
/// </summary>
[CustomEditor(typeof(ObjectPoolProfilerProvider))]
public class ObjectPoolProfiler : Editor
{
    private IPoolProfiler _poolProfiler;

    /// <summary>
    /// Key - manager
    /// Value - profiler
    /// </summary>
    private readonly Dictionary<Type, Type> _poolProfilerTypes = new Dictionary<Type, Type>
    {
        {typeof(ObjectPoolKeyManager), typeof(KeyManagerProfiler)},
        {typeof(ObjectPoolSeparateKeyManager), typeof(SeparateKeyManagerProfiler)},
        {typeof(ObjectPoolTypeManager), typeof(TypeManagerProfiler)},
        {typeof(ObjectPoolSeparateTypeManager), typeof(SeparateTypeManagerProfiler)},
    };
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var root = CreateInspectorGUI();
        _poolProfiler?.DrawStatus(root);
    }
    
    private void OnEnable()
    {
        var provider = target as ObjectPoolProfilerProvider;
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
        var provider = target as ObjectPoolProfilerProvider;
        if (provider == null)
            return;
        provider.OnPoolUpdate -= SetupPoolProfiler;
        if (_poolProfiler != null)
            provider.OnPoolStructureUpdate -= _poolProfiler.OnPoolDataUpdated;
        provider.OnPoolStructureUpdate -= Repaint;
    }

    private void SetupPoolProfiler(ObjectPoolProfilerProvider poolProfilerProvider)
    {
        if (poolProfilerProvider == null || poolProfilerProvider.PoolManager == null || poolProfilerProvider.Pool == null)
        {
            Debug.LogError("Trying assign null pool entity");
            return;
        }
        
        var managerType = poolProfilerProvider.PoolManager.GetType();
        if (_poolProfilerTypes.TryGetValue(managerType, out var profilerType) == false)
        {
            Debug.LogError($"For {managerType.Name} type profiler not found");
            return;
        }
        
        _poolProfiler = Activator.CreateInstance(profilerType, poolProfilerProvider.Pool) as IPoolProfiler;
    }
}
}