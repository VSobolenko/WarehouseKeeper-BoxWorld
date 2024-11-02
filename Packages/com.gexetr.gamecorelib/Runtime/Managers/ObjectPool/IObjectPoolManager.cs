using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Pools
{
public interface IObjectPoolManager : IObjectPoolRecyclable
{
    /// <summary>
    /// Notify about availability, without preparation or addition
    /// </summary>
    /// <param name="prefab">Object prefab or instance</param>
    /// <param name="count">Expected quantity to add</param>
    /// <typeparam name="T">Object type</typeparam>
    ///void Warn<T>(T prefab, int count) where T : Object, IPoolable;

    /// <summary>
    /// Add ready instances to pool
    /// </summary>
    /// <param name="prefab">Object prefab or instance</param>
    /// <param name="count">Quantity to add</param>
    /// <typeparam name="T">Object type</typeparam>
    void Prepare<T>(T prefab, int count) where T : Object, IPoolable;

    Task PrepareAsync<T>(T prefab, int count, CancellationToken token = default) where T : Object, IPoolable;

    /// <summary>
    /// Get object instance from pool
    /// </summary>
    /// <param name="prefab">Object prefab or instance</param>
    /// <param name="position">Position for the object</param>
    /// <param name="rotation">Orientation of the object</param>
    /// <param name="parent">Parent that will be assigned to the object</param>
    /// <typeparam name="T">Object type</typeparam>
    /// <returns>The instantiated clone</returns>
    T Get<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent) where T : Object, IPoolable;

    T Get<T>(T prefab) where T : Object, IPoolable;

    T Get<T>(T prefab, Transform parent) where T : Object, IPoolable;

    T Get<T>(T prefab, Vector3 position, Quaternion rotation) where T : Object, IPoolable;
}
}