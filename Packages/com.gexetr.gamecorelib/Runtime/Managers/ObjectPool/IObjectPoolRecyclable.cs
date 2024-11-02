using UnityEngine;

namespace Game.Pools
{
public interface IObjectPoolRecyclable
{
    /// <summary>
    /// Return object to pool
    /// </summary>
    /// <param name="prefabInstance">Object instance</param>
    /// <typeparam name="T">Object type</typeparam>
    void Release<T>(T prefabInstance) where T : Object, IPoolable;
}
}