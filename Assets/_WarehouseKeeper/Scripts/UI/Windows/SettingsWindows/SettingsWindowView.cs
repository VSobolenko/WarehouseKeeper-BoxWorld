using System;
using Game.GUI.Windows;
using UnityEngine;
using UnityEngine.UI;
using WarehouseKeeper._WarehouseKeeper.Scripts.UI.Windows.SettingsWindows.Components;
using WarehouseKeeper.UI.Windows.SettingsWindows.Components;

namespace WarehouseKeeper.UI.Windows.SettingsWindows
{
public sealed class SettingsWindowView : WindowUI
{
    [field: SerializeField] public Toggle SoundToggle { get; private set; }
    [field: SerializeField] public Toggle MusicToggle { get; private set; }
    
    [SerializeField] private SettingsWindowButton[] windowButtons;
    [SerializeField] private SettingsWindowToggle[] windowToggles;

    public event Action<SettingsWindowAction> OnWindowAction;
    public event Action<SettingsWindowAction, bool> OnWindowToggleAction;

    private void Start()
    {
        foreach (var button in windowButtons)
            button.OnClickButton += ClickWindowAction;
        foreach (var toggle in windowToggles)
            toggle.OnClickToggle += ClickWindowToggleAction;
    }

    private void OnDestroy()
    {
        foreach (var button in windowButtons)
            button.OnClickButton -= ClickWindowAction;
        foreach (var toggle in windowToggles)
            toggle.OnClickToggle -= ClickWindowToggleAction;
    }

    private void ClickWindowAction(SettingsWindowAction action) => OnWindowAction?.Invoke(action);
    private void ClickWindowToggleAction(SettingsWindowAction action, bool value) => OnWindowToggleAction?.Invoke(action, value);
    
#if UNITY_EDITOR
    
    [ContextMenu("Collect window buttons"),]
    private void CollectWindowButtons()
    {
        windowButtons = GetComponentsInChildren<SettingsWindowButton>(true);

        UnityEditor.EditorUtility.SetDirty(this);
    }
    
    [ContextMenu("Collect window toggles"),]
    private void CollectWindowToggles()
    {
        windowToggles = GetComponentsInChildren<SettingsWindowToggle>(true);

        UnityEditor.EditorUtility.SetDirty(this);
    }
    
#endif
}
}