using System;
using UnityEngine;

namespace WarehouseKeeper.Directors.Game.UserMeta.UserSkins
{
[CreateAssetMenu(fileName = "UserSkinsData", menuName = "Warehouse Keeper/User Meta/User Skins", order = 0)]
internal class UserSkinScriptableObject : BaseAppearanceScriptableObject<UserSkinData>
{
}

[Serializable]
internal struct UserSkinData
{
    public string keyId;
    
    public bool accessible;
    public string addressableKey;
    public Sprite icon;
}
}