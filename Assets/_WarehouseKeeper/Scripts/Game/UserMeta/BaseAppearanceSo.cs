using UnityEngine;

namespace WarehouseKeeper.Directors.Game.UserMeta.UserSkins
{
public class BaseAppearanceSo<T> : ScriptableObject where T : struct
{
    [field: SerializeField] public string StartedId { get; private set; }
    [field: SerializeField] public string[] DefaultKeysId { get; private set; }
    [field: SerializeField] public T[] Items { get; private set; }
}
}