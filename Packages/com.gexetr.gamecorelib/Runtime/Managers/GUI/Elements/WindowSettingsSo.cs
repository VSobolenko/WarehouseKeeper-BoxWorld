using System;
using DG.Tweening;
using Game.InternalData;
using UnityEngine;

namespace Game.GUI.Windows
{
[Serializable]
internal class WindowSettings
{
    [SerializeField] private float _transitionMoveDuration = .5f;
    [SerializeField] private Ease _moveType = Ease.Linear;

    public float TransitionMoveDuration => _transitionMoveDuration;
    public Ease MoveType => _moveType;
}

[CreateAssetMenu(fileName = nameof(WindowSettings), menuName = GameData.EditorName +"/Window Settings", order = 3)]
internal class WindowSettingsSo : ScriptableObject
{
    [Header("Default Transition")]
    public WindowSettings windowSettings;
}
}