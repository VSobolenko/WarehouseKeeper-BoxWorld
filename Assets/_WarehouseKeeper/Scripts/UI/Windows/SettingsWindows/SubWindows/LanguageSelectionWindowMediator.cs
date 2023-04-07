using System;
using Game;
using Game.GUI.Windows;
using Game.Localizations;
using WarehouseKeeper.Directors.UI.Windows;
using WarehouseKeeper.UI.Windows.SettingsWindows.SubWindows.Elements;

namespace WarehouseKeeper.UI.Windows.SettingsWindows.SubWindows
{
internal class LanguageSelectionWindowMediator : BaseMediator<LanguageSelectionWindowView>
{
    private readonly ILocalizationManager _localizationManager;
    private readonly WindowsDirector _windowsDirector;
    
    public LanguageSelectionWindowMediator(LanguageSelectionWindowView window, ILocalizationManager localizationManager, WindowsDirector windowsDirector) : base(window)
    {
        _localizationManager = localizationManager;
        _windowsDirector = windowsDirector;
    }

    public override void OnInitialize()
    {
        window.OnWindowAction += ProceedButtonAction;
        window.OnWindowLanguage += ProceedLanguageAction;
    }

    public override void OnDestroy()
    {
        window.OnWindowAction -= ProceedButtonAction;
        window.OnWindowLanguage -= ProceedLanguageAction;
    }
    
    #region Button event handler

    private void ProceedLanguageAction(LanguageType language)
    {
        if (_localizationManager.ActiveLanguage == language)
            return;
        
        Log.Info($"Select language: {language}");
        _localizationManager.SetLanguage(language);
        ClickCancelButton();
    }

    private void ProceedButtonAction(LanguageSelectionWindowAction action)
    {
        switch (action)
        {
            case LanguageSelectionWindowAction.OnClickCancel:
                ClickCancelButton();
                break;
            default:
                Log.Error($"Unknown action {action}");
                break;
            
        }
    }
    
    private void ClickCancelButton()
    {
        _windowsDirector.CloseWindow(this);
    }
    
    #endregion
}
}