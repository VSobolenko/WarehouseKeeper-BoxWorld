using System;
using UnityEngine;
using WarehouseKeeper.EditorScripts;

namespace WarehouseKeeper.Directors.Game.UserMeta.UserSkins
{
[CreateAssetMenu(fileName = "BoxSkinsData", menuName = EditorGameData.EditorName + "/User Meta/Box Skins", order = 0)]
internal class BoxSkinSo : BaseAppearanceSo<BoxSkinData>
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