using System;
using UnityEngine;
using WarehouseKeeper.Gui.Windows;
using WarehouseKeeper.UI.Windows.LevelSelections;
using WarehouseKeeper.UI.Windows.LevelSelections.Components;

namespace WarehouseKeeper.UI.Windows.LevelSelection
{
internal sealed class LevelSelectionWindowView : WindowUI
{
    [field: SerializeField] public PlayerResourcesView PlayerResourcesView { get; private set; }
    [field: SerializeField] public Transform ItemsRoot { get; private set; }
    [SerializeField] private LevelSelectionWindowButton[] windowButtons;

    public event Action<LevelSelectionWindowAction> OnWindowAction;

    private void Start()
    {
        foreach (var button in windowButtons)
            button.OnClickButton += ClickWindowAction;
    }
    
    private void OnDestroy()
    {
        foreach (var button in windowButtons)
            button.OnClickButton -= ClickWindowAction;
    }

    private void ClickWindowAction(LevelSelectionWindowAction action)
    {
        OnWindowAction?.Invoke(action);
    }
    
#if UNITY_EDITOR
    
    [ContextMenu("Collect window buttons"),]
    private void CollectWindowButtons()
    {
        windowButtons = GetComponentsInChildren<LevelSelectionWindowButton>(true);

        UnityEditor.EditorUtility.SetDirty(this);
    }
    
#endif
}
}