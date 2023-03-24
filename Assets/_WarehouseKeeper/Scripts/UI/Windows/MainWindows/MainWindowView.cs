using System;
using UnityEngine;
using WarehouseKeeper.Gui.Windows;
using WarehouseKeeper.UI.Windows.MainWindows.Components;

namespace WarehouseKeeper.UI.Windows.MainWindows
{
internal sealed class MainWindowView : WindowUI
{
    [field: SerializeField] public PlayerResourcesView PlayerResourcesView { get; private set; }
    [SerializeField] private MainWindowButton[] windowButtons;

    public event Action<MainWindowAction> OnWindowAction;

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
    
    private void ClickWindowAction(MainWindowAction action)
    {
        OnWindowAction?.Invoke(action);
    }
    
#if UNITY_EDITOR
    
    [ContextMenu("Collect window buttons"),]
    private void CollectWindowButtons()
    {
        windowButtons = GetComponentsInChildren<MainWindowButton>(true);

        UnityEditor.EditorUtility.SetDirty(this);
    }
    
#endif
}
}