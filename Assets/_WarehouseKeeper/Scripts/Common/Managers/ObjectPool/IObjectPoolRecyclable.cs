using UnityEngine;

namespace WarehouseKeeper.Pools
{
internal interface IObjectPoolRecyclable
{
    /// <summary>
    /// Return object to pool
    /// </summary>
    /// <param name="prefab">Object instance</param>
    /// <typeparam name="T">Object type</typeparam>
    void Release<T>(T prefab) where T : Object, IPoolable;
}
}