using UnityEngine;

namespace WarehouseKeeper.Pools.Managers
{
internal interface IObjectPoolRecyclable
{
    void Release<T>(T prefab) where T : Object, IPoolable;
}
}