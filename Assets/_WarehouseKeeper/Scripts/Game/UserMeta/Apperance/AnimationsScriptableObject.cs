using System;
using UnityEngine;

namespace WarehouseKeeper.Directors.Game.UserMeta.UserSkins
{
[CreateAssetMenu(fileName = "AnimationsData", menuName = "Warehouse Keeper/User Meta/Animations", order = 0)]
internal class AnimationsScriptableObject : BaseAppearanceScriptableObject<AnimationData>
{
}

[Serializable]
internal struct AnimationData
{
    public string keyId;
    
    public bool accessible;
    public string addressableKey;
    public Sprite icon;
}
}