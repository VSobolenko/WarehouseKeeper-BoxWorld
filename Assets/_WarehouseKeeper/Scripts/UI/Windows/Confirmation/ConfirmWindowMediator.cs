using System;
using Game;
using Game.GUI.Windows;
using Game.Localizations;
using WarehouseKeeper.UI.Windows.ConfirmationWindows.Components;

namespace WarehouseKeeper.UI.Windows.ConfirmationWindows
{
internal class ConfirmWindowMediator : BaseMediator<ConfirmWindowView>
{
    private readonly ILocalizationManager _localizationManager;
    private Action _agree;
    private Action _disagree;
    
    public ConfirmWindowMediator(ConfirmWindowView window, ILocalizationManager localizationManager) : base(window)
    {
        _localizationManager = localizationManager;
    }

    public override void OnInitialize()
    {
        window.OnWindowAction += ProceedButtonAction;
    }

    public override void OnDestroy()
    {
        window.OnWindowAction -= ProceedButtonAction;
    }
    
    public void SetupWindow(string descriptionKey, string agreeKey, string disagreeKey,
                            Action agreeAction, Action disagreeAction)
    {
        var description = _localizationManager.Localize(descriptionKey, descriptionKey);
        var agree = _localizationManager.Localize(agreeKey, agreeKey);
        var disagree = _localizationManager.Localize(disagreeKey, disagreeKey);

        _agree = agreeAction;
        _disagree = disagreeAction;

        window.Setup(description, agree, disagree);
    }

    #region Button event handler

    private void ProceedButtonAction(ConfirmWindowAction action)
    {
        switch (action)
        {
            case ConfirmWindowAction.OnClickAgree:
                _agree?.Invoke();
                break;
            case ConfirmWindowAction.OnClickDisagree:
                _disagree?.Invoke();
                break;
            case ConfirmWindowAction.OnClickCancel:
                _disagree?.Invoke();
                break;
            default:
                Log.WriteError($"Unknown action {action}");
                break;
        }
    }

    #endregion
}
}
