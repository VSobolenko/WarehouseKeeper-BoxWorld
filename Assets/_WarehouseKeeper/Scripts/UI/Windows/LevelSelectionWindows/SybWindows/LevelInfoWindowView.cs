using System;
using TMPro;
using UnityEngine;
using WarehouseKeeper._WarehouseKeeper.Scripts.UI.Windows.LevelSelectionWindows.SybWindows.Components;
using WarehouseKeeper.Gui.Windows;
using WarehouseKeeper.UI.Windows;

namespace WarehouseKeeper._WarehouseKeeper.Scripts.UI.Windows.LevelSelectionWindows.SybWindows
{
public class LevelInfoWindowView : WindowUI
{
    [field: SerializeField] public TextMeshProUGUI LevelNumber { get; private set; }
    [field: SerializeField] public TextMeshProUGUI CountMoves { get; private set; }
    [field: SerializeField] public TextMeshProUGUI BestMoves { get; private set; }
    [field: SerializeField] public TextMeshProUGUI CountPushes { get; private set; }
    [field: SerializeField] public TextMeshProUGUI BestPushes { get; private set; }
    [field: SerializeField] public TextMeshProUGUI TimeSpent { get; private set; }
    [field: SerializeField] public TextMeshProUGUI BestTime { get; private set; }
    [field: SerializeField] public StarsView Stars { get; private set; }
    [SerializeField] private LevelInfoWindowButton[] _windowButtons;

    public event Action<LevelInfoWindowAction> OnWindowAction;

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

    private void ClickWindowAction(LevelInfoWindowAction action)
    {
        OnWindowAction?.Invoke(action);
    }
    
#if UNITY_EDITOR
    
    [UnityEngine.ContextMenu("Collect window buttons"),]
    private void CollectWindowButtons()
    {
        _windowButtons = GetComponentsInChildren<LevelInfoWindowButton>(true);

        UnityEditor.EditorUtility.SetDirty(this);
    }
    
#endif
}
}