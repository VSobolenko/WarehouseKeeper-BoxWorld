using System;
using System.Linq;
using UnityEngine;
using WarehouseKeeper.Directors.Game;
using WarehouseKeeper.Directors.Game.Analytics.Signals;
using WarehouseKeeper.Directors.Game.Audio;
using WarehouseKeeper.Directors.Game.UserResources;
using WarehouseKeeper.Directors.UI.Windows;
using WarehouseKeeper.Extension;
using WarehouseKeeper.Gui.Windows.Mediators;
using WarehouseKeeper.Levels;
using WarehouseKeeper.Localizations;
using WarehouseKeeper.StaticData;
using Zenject;

namespace WarehouseKeeper.UI.Windows.MainWindows
{
internal class MainWindowMediator : BaseMediator<MainWindowView>
{
    private readonly WindowsDirector _windowsDirector;
    private readonly GameDirector _gameDirector;
    private readonly AudioDirector _audioDirector;
    private readonly PlayerResourcesDirector _playerResourcesDirector;
    private readonly LevelRepositoryDirector _levelRepositoryDirector;
    private readonly ILocalizationManager _localizationManager;
    private readonly SignalBus _signalBus;
    
    public MainWindowMediator(MainWindowView window, 
                              WindowsDirector windowsDirector,
                              GameDirector gameDirector, 
                              AudioDirector audioDirector, ILocalizationManager localizationManager, PlayerResourcesDirector playerResourcesDirector, LevelRepositoryDirector levelRepositoryDirector, SignalBus signalBus) : base(window)
    {
        _windowsDirector = windowsDirector;
        _gameDirector = gameDirector;
        _audioDirector = audioDirector;
        _localizationManager = localizationManager;
        _playerResourcesDirector = playerResourcesDirector;
        _levelRepositoryDirector = levelRepositoryDirector;
        _signalBus = signalBus;
    }

    public override void OnInitialize()
    {
        window.OnWindowAction += ProceedButtonAction;
        _audioDirector.PlayLobbyBackground();
        window.PlayerResourcesView.AutoInitialize(_playerResourcesDirector, _localizationManager);
    }

    public override void OnDestroy()
    {
        _audioDirector.StopLobbyBackground();
        window.OnWindowAction -= ProceedButtonAction;
        window.PlayerResourcesView.AutoDispose();
    }
    
    #region Button event handler

    private void ProceedButtonAction(MainWindowAction action)
    {
        switch (action)
        {
            case MainWindowAction.OnClickPlay:
                ClickPlay();
                break;
            case MainWindowAction.OnClickSelectLevel:
                _windowsDirector.OpenLevelSelectionWindow();
                break;
            case MainWindowAction.OnClickShop:
                _windowsDirector.OpenShopWindow();
                break;
            case MainWindowAction.OnClickSettings:
                _windowsDirector.OpenSettingsWindow();
                break;
            case MainWindowAction.OnClickAppearance:
                _windowsDirector.OpenAppearanceWindow();
                break;
            case MainWindowAction.OnClickCloseYourself:
                _windowsDirector.CloseWindow(this);
                break;
            case MainWindowAction.OnClickDiscord:
                ClickOpenDiscord();
                break;
            case MainWindowAction.OnClickQuit:
                ClickQuite();
                break;
            default:
                Log.WriteError($"Unknown action {action}");
                break;
        }
    }

    private void ClickPlay()
    {
        var lastLevel = _levelRepositoryDirector.GetLevelsData()?.LastOrDefault();
        if (lastLevel == null)
        {
            Log.WriteError("Can't load level");
            return;
        }
        
        _signalBus.Fire(new LevelStart
        {
            levelId = lastLevel.Id,
        });
        
        _gameDirector.StartLevel(lastLevel.Id);
        _windowsDirector.CloseWindow(this);
    }

    private void ClickQuite()
    {
#if UNITY_EDITOR
        Log.Write("Game quit");
#endif
        Application.Quit();
    }
    
    private void ClickOpenDiscord()
    {
        Application.OpenURL(GameData.DiscordUrl);    
    }
    
    #endregion
}
}