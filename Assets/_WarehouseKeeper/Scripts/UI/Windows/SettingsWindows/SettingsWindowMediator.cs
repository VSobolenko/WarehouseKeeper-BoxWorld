using System;
using WarehouseKeeper.Audio;
using WarehouseKeeper.Directors.Game.Analytics.Signals;
using WarehouseKeeper.Directors.Game.UserResources;
using WarehouseKeeper.Directors.UI.Windows;
using WarehouseKeeper.Extension;
using WarehouseKeeper.Gui.Windows.Mediators;
using WarehouseKeeper.Levels;
using WarehouseKeeper.UI.Windows.MainWindows;
using Zenject;

namespace WarehouseKeeper.UI.Windows.SettingsWindows
{
internal class SettingsWindowMediator : BaseMediator<SettingsWindowView>
{
    private readonly WindowsDirector _windowsDirector;
    private readonly IAudioManager _audioManager;
    private readonly LevelRepositoryDirector _levelRepositoryDirector;
    private readonly PlayerResourcesDirector _playerResourcesDirector;
    private readonly SignalBus _signalBus;

    public SettingsWindowMediator(SettingsWindowView window,
                                  WindowsDirector windowsDirector, IAudioManager audioManager, LevelRepositoryDirector levelRepositoryDirector, PlayerResourcesDirector playerResourcesDirector, SignalBus signalBus) : base(window)
    {
        _windowsDirector = windowsDirector;
        _audioManager = audioManager;
        _levelRepositoryDirector = levelRepositoryDirector;
        _playerResourcesDirector = playerResourcesDirector;
        _signalBus = signalBus;
    }

    public override void OnInitialize()
    {
        window.OnWindowAction += ProceedButtonAction;
        window.OnWindowToggleAction += ProceedToggleAction;
        window.MusicToggle.isOn = _audioManager.MusicEnabled;
        window.SoundToggle.isOn = _audioManager.SoundEnabled;
    }

    public override void OnDestroy()
    {
        window.OnWindowAction -= ProceedButtonAction;
        window.OnWindowToggleAction -= ProceedToggleAction;
    }

    #region Button&Toggle event handler

    private void ProceedButtonAction(SettingsWindowAction action)
    {
        switch (action)
        {
            case SettingsWindowAction.OnClickCloseYourself:
                _windowsDirector.CloseWindow(this);
                break;
            case SettingsWindowAction.OnClickResetProgress:
                ClickResetProgress();
                break;
            case SettingsWindowAction.OnClickSelectLanguage:
                ClickSelectLanguage();
                break;
            default:
                Log.WriteError($"Unknown action {action}");
                break;
        }
    }

    private void ProceedToggleAction(SettingsWindowAction action, bool value)
    {
        switch (action)
        {
            case SettingsWindowAction.OnClickMusic:
                _audioManager.MusicEnabled = value;
                break;
            case SettingsWindowAction.OnClickSound:
                _audioManager.SoundEnabled = value;
                break;
            default:
                Log.WriteError($"Unknown action {action}");
                break;
        }
    }

    private void ClickResetProgress()
    {
        const string descriptionKey = "settings_resetProgress_header";
        const string agreeKey = "settings_resetProgress_agreeKey";
        const string disagreeKey = "settings_resetProgress_disagreeKey";

        _windowsDirector.OpenConfirmWindow(mediator =>
        {
            mediator.SetupWindow(descriptionKey, agreeKey, disagreeKey, () =>
            {
                //Reset level progress
                var count = _levelRepositoryDirector.ResetAllLevelData();
                _levelRepositoryDirector.CreateEmptyData(0);

                //Reset user progress
                var adsDisable = _playerResourcesDirector.UserData.AdsDisable;
                _playerResourcesDirector.Reset();

                //Save ads purchase
                if (adsDisable)
                {
                    _playerResourcesDirector.UpdateData(data =>
                    {
                        data.AdsDisable = true;
                    });
                }
                
                //Analytics
                if (count > 0)
                    _signalBus.Fire(new ResetProgress {countUnlockLevels = count});

                _windowsDirector.CloseWindow(mediator);
            }, () =>
            {
                _windowsDirector.CloseWindow(mediator);
            });
        });
    }

    private void ClickSelectLanguage()
    {
        _windowsDirector.OpenLanguageSelectionWindow();
    }

    #endregion
}
}