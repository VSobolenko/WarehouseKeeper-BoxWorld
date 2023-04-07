using System;
using UnityEngine;
using WarehouseKeeper.EditorScripts;

namespace WarehouseKeeper.Directors.Game.UserMeta.UserSkins
{
[CreateAssetMenu(fileName = "UserSkinsData", menuName = EditorGameData.EditorName + "/User Meta/User Skins", order = 0)]
internal class UserSkinSo : BaseAppearanceSo<UserSkinData>
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