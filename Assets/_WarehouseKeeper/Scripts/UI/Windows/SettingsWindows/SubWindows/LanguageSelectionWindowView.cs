using System;
using Game.GUI.Windows;
using Game.Localizations;
using UnityEngine;
using WarehouseKeeper.UI.Windows.SettingsWindows.SubWindows.Elements;

namespace WarehouseKeeper.UI.Windows.SettingsWindows.SubWindows
{
internal class LanguageSelectionWindowView : WindowUI
{
    [SerializeField] private LanguageSelectionWindowLanguageButton[] _languages;
    [SerializeField] private LanguageSelectionWindowButton[] _windowButtons;

    public event Action<LanguageSelectionWindowAction> OnWindowAction;
    public event Action<LanguageType> OnWindowLanguage;

    private void Start()
    {
        foreach (var button in _windowButtons)
            button.OnClickButton += ClickWindowAction;
        foreach (var button in _languages)
            button.OnClickButton += ClickWindowLanguage;
    }

    private void OnDestroy()
    {
        foreach (var button in _windowButtons)
            button.OnClickButton -= ClickWindowAction;
        foreach (var button in _languages)
            button.OnClickButton -= ClickWindowLanguage;
    }

    private void ClickWindowAction(LanguageSelectionWindowAction action) => OnWindowAction?.Invoke(action);
    private void ClickWindowLanguage(LanguageType language) => OnWindowLanguage?.Invoke(language);
    
#if UNITY_EDITOR
    
    [ContextMenu("Collect window buttons"),]
    private void CollectWindowButtons()
    {
        _windowButtons = GetComponentsInChildren<LanguageSelectionWindowButton>(true);

        UnityEditor.EditorUtility.SetDirty(this);
    }

    [ContextMenu("Collect window languages"),]
    private void CollectWindowLanguages()
    {
        _languages = GetComponentsInChildren<LanguageSelectionWindowLanguageButton>(true);

        UnityEditor.EditorUtility.SetDirty(this);
    }
    
#endif
}
}