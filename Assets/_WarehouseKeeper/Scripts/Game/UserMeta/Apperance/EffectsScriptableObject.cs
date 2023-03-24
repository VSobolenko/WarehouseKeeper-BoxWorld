using System;
using UnityEngine;

namespace WarehouseKeeper.Directors.Game.UserMeta.UserSkins
{
[CreateAssetMenu(fileName = "EffectsData", menuName = "Warehouse Keeper/User Meta/Effects", order = 0)]
internal class EffectsScriptableObject : BaseAppearanceScriptableObject<EffectsData>
{
}

[Serializable]
internal struct EffectsData
{
    public string keyId;
    
    public bool accessible;
    public string addressableKey;
    public Sprite icon;
}
}