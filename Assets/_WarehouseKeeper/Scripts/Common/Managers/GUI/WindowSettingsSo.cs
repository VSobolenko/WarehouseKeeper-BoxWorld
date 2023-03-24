using System;
using DG.Tweening;
using UnityEngine;

namespace WarehouseKeeper.Gui.Windows
{
[Serializable]
internal class WindowSettings
{
    [Header("Transition"), SerializeField] private float transitionMoveDuration = .5f;
    [SerializeField] private Ease moveType = Ease.Linear;

    public float TransitionMoveDuration => transitionMoveDuration;
    public Ease MoveType => moveType;
}

[CreateAssetMenu(fileName = "WindowSettings", menuName = "Warehouse Keeper/Window Settings", order = 3)]
internal class WindowSettingsSo : ScriptableObject
{
    public WindowSettings windowSettings;
}
}