using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace WarehouseKeeper.Pools.Managers
{
internal interface IObjectPoolManager : IObjectPoolRecyclable
{
    void Prepare<T>(T prefab, int count) where T : Object, IPoolable;
    
    Task PrepareAsync<T>(T prefab, int count, CancellationToken token = default) where T : Object, IPoolable;
    
    T Get<T>(T prefab) where T : Object, IPoolable;
    
    T Get<T>(T prefab, Vector3 position, Quaternion rotation) where T : Object, IPoolable;
    
    T Get<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent) where T : Object, IPoolable;
    
    T Get<T>(T prefab, Transform parent) where T : Object, IPoolable;
}
}