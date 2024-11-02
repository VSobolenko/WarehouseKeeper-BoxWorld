using System;
using Game.InternalData;
using UnityEngine;

namespace Game.Inputs
{
[Serializable]
public class InputSettings
{
    [SerializeField] private Vector2 inputResolution = new Vector2(1280, 720);
    [Header("Swipe"), SerializeField] private float swipeMinimumDistance = .2f;
    [SerializeField] private float directionThreshold = 0.9f;
    [SerializeField] private double maxSwipeTimeSeconds = 1f;

    public float SwipeMinimumDistance => Mathf.Min(Screen.width, Screen.height) * swipeMinimumDistance /
                                         Mathf.Min(inputResolution.x, inputResolution.y);
    public float DirectionThreshold => directionThreshold;
    public double MaxSwipeTimeSeconds => maxSwipeTimeSeconds;
}

[CreateAssetMenu(fileName = nameof(InputSettings), menuName = GameData.EditorName + "/Input Settings", order = 2)]
internal class InputSettingsSo : ScriptableObject
{
    public InputSettings inputSettings;
}
}