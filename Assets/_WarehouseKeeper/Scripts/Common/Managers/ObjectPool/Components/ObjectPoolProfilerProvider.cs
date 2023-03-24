using System;
using System.Collections.Generic;
using UnityEngine;

namespace WarehouseKeeper.Pools.Implementation
{
public class ObjectPoolProfilerProvider : MonoBehaviour
{
    public object Pool { get; private set; }
    public object PoolManager { get; private set; }
    public event Action<ObjectPoolProfilerProvider> OnPoolUpdate;
    public event Action OnPoolStructureUpdate;

    public void AssignPool(object poolManager, object pool)
    {
        PoolManager = poolManager;
        Pool = pool;
        OnPoolUpdate?.Invoke(this);
    }

    public void UpdateProfiler()
    {
        OnPoolStructureUpdate?.Invoke();
    }
}
}