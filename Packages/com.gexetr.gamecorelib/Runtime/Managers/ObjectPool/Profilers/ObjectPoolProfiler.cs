using System;
using UnityEngine;

namespace Game.Pools
{
internal class ObjectPoolProfiler : MonoBehaviour
{
    public object PoolContainer { get; private set; }
    public object PoolManager { get; private set; }
    public event Action<ObjectPoolProfiler> OnPoolUpdate;
    public event Action OnPoolStructureUpdate;

    public void AssignPool(IObjectPoolManager poolManager, object poolContainer)
    {
        PoolManager = poolManager;
        PoolContainer = poolContainer;
        OnPoolUpdate?.Invoke(this);
    }

    public void UpdateProfiler()
    {
        OnPoolStructureUpdate?.Invoke();
    }
}
}