using System;
using UnityEngine;

namespace WarehouseKeeper.Directors.Game.UserMeta.UserSkins
{
[CreateAssetMenu(fileName = "BoxSkinsData", menuName = "Warehouse Keeper/User Meta/Box Skins", order = 0)]
internal class BoxSkinScriptableObject : BaseAppearanceScriptableObject<BoxSkinData>
{
}

[Serializable]
internal struct BoxSkinData
{
    public string keyId;
    
    public bool accessible;
    public string addressableKey;
    public Sprite icon;
}
}