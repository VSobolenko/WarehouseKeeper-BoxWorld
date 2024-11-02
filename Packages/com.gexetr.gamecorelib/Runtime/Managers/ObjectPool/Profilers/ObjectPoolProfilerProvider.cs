using UnityEngine;

namespace Game.Pools
{
internal class ObjectPoolProfilerProvider
{
    private readonly Transform _root;
    private ObjectPoolProfiler _profiler;

    public ObjectPoolProfilerProvider(Transform root)
    {
        _root = root;
    }

    public ObjectPoolProfilerProvider Initialize(IObjectPoolManager poolManager, object poolContainer)
    {
        if (_root == null)
        {
            Log.Warning("Cannot add profiler without root object");

            return this;
        }

        if (_profiler == null)
            _profiler = _root.gameObject.AddComponent<ObjectPoolProfiler>();

        _profiler.AssignPool(poolManager, poolContainer);

        return this;
    }

    public ObjectPoolProfilerProvider AssignProfilerView(ObjectPoolProfiler profiler)
    {
        _profiler = profiler;

        return this;
    }

    public void Update() => _profiler.UpdateProfiler();
}
}