using System;
using UnityEngine;
using WarehouseKeeper.EditorScripts;

namespace WarehouseKeeper.Directors.Game.UserMeta.UserSkins
{
[CreateAssetMenu(fileName = "AnimationsData", menuName = EditorGameData.EditorName + "/User Meta/Animations", order = 0)]
internal class AnimationsSo : BaseAppearanceSo<AnimationData>
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