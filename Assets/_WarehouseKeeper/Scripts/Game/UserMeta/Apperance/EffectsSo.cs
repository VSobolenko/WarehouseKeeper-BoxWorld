using System;
using UnityEngine;
using WarehouseKeeper.EditorScripts;

namespace WarehouseKeeper.Directors.Game.UserMeta.UserSkins
{
[CreateAssetMenu(fileName = "EffectsData", menuName = EditorGameData.EditorName + "/User Meta/Effects", order = 0)]
internal class EffectsSo : BaseAppearanceSo<EffectsData>
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