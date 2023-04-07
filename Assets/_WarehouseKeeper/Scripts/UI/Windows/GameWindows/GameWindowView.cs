using System;
using Game.GUI.Windows;
using TMPro;
using UnityEngine;
using WarehouseKeeper._WarehouseKeeper.Scripts.UI.Windows.GameWindows.Components;
using WarehouseKeeper.UI.Windows.GameWindows.Components;

namespace WarehouseKeeper.UI.Windows.GameWindows
{
public class GameWindowView : WindowUI
{
    [field: SerializeField] public TextMeshProUGUI LevelNumberText { get; private set; }
    [field: SerializeField] public TextMeshProUGUI Time { get; private set; }

    [SerializeField] private TextMeshProUGUI _movesTargetText;
    [SerializeField] private TextMeshProUGUI _hintCounter;
    [SerializeField] private StarsView _stars;
    [field: SerializeField] public GameHintUI Hint { get; private set; }
    [field: SerializeField] public GameWindowFadeTransition[] FadedComponents { get; private set; }
    [SerializeField] private GameWindowButton[] _windowButtons;

    public event Action<GameWindowAction> OnWindowAction;

    private void Start()
    {
        foreach (var button in _windowButtons)
            button.OnClickButton += ClickWindowAction;
    }

    private void OnDestroy()
    {
        foreach (var button in _windowButtons)
            button.OnClickButton -= ClickWindowAction;
    }

    public void UpdateStatistics(int countMoves, int targetMoves, int countStars)
    {
        _movesTargetText.text = $"{countMoves}/{(countStars > 1 ? targetMoves : "∞")}";
        _stars.StaticSetup(countStars);
    }
    
    public void UpdateHintCounter(int countHints)
    {
        _hintCounter.text = countHints.ToString();
    }
    
    private void ClickWindowAction(GameWindowAction action) => OnWindowAction?.Invoke(action);
    
#if UNITY_EDITOR
    
    [ContextMenu("Collect window buttons"),]
    private void CollectWindowButtons()
    {
        _windowButtons = GetComponentsInChildren<GameWindowButton>(true);

        UnityEditor.EditorUtility.SetDirty(this);
    }
    
    [ContextMenu("Collect fade transition components"),]
    private void CollectWindowDisableComponents()
    {
        FadedComponents = GetComponentsInChildren<GameWindowFadeTransition>(true);

        UnityEditor.EditorUtility.SetDirty(this);
    }
    
#endif
}
}