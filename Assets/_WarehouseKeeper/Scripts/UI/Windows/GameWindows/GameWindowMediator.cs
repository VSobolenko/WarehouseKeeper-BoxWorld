using System;
using System.Text;
using Game;
using Game.GUI.Windows;
using Game.Localizations;
using WarehouseKeeper.Directors.Game;
using WarehouseKeeper.Directors.Game.Analytics.Signals;
using WarehouseKeeper.Directors.Game.Audio;
using WarehouseKeeper.Directors.Game.UserResources;
using WarehouseKeeper.Directors.UI.Windows;
using WarehouseKeeper.Levels;
using Zenject;

namespace WarehouseKeeper.UI.Windows.GameWindows
{
internal class GameWindowMediator : BaseMediator<GameWindowView>
{
    private readonly WindowsDirector _windowsDirector;
    private readonly LevelDirector _levelDirector;
    private readonly LevelProgressDirector _progressDirector;
    private readonly GameDirector _gameDirector;
    private readonly ILocalizationManager _localization;
    private readonly PlayerResourcesDirector _playerResourcesDirector;
    private readonly AudioDirector _audioDirector;
    private readonly SignalBus _signalBus;

    public GameWindowMediator(GameWindowView window, 
                              WindowsDirector windowsDirector, 
                              LevelDirector levelDirector, 
                              GameDirector gameDirector,
                              LevelProgressDirector progressDirector,
                              ILocalizationManager localization, PlayerResourcesDirector playerResourcesDirector, AudioDirector audioDirector, SignalBus signalBus) : base(window)
    {
        _windowsDirector = windowsDirector;
        _gameDirector = gameDirector;
        _levelDirector = levelDirector;
        _localization = localization;
        _playerResourcesDirector = playerResourcesDirector;
        _audioDirector = audioDirector;
        _signalBus = signalBus;
        _progressDirector = progressDirector;
    }

    public override void OnInitialize()
    {
        window.OnWindowAction += ProceedButtonAction;
        _levelDirector.OnUpdateDynamicData += UpdateDynamicData;
        _levelDirector.OnUpdateStaticData += UpdateStaticData;
        _playerResourcesDirector.OnUpdateUserData += UpdateStaticWindowData;
        UpdateStaticWindowData();
    }

    public override void OnShow()
    {
        _gameDirector.Resume();
    }

    public override void OnDestroy()
    {
        window.OnWindowAction -= ProceedButtonAction;
        _levelDirector.OnUpdateDynamicData -= UpdateDynamicData;
        _levelDirector.OnUpdateStaticData -= UpdateStaticData;
        _playerResourcesDirector.OnUpdateUserData -= UpdateStaticWindowData;
        _audioDirector.StopGameBackground();
    }

    public void UpdateWindowData()
    {
        UpdateStaticData(_levelDirector.Statistics);
        UpdateDynamicData(_levelDirector.Statistics);
    }

    private void UpdateStaticWindowData()
    {
        window.UpdateHintCounter(_playerResourcesDirector.UserData.Hints.Value);
    }
    
    private void UpdateStaticData(LevelStatistics statistics)
    {
        window.LevelNumberText.text = _localization.LocalizeFormat("game_level", _levelDirector.ActiveLevel.LevelId + 1);

        var activeStars = _progressDirector.GetActiveStars();
        var activeTarget = _progressDirector.GetActiveTarget();
        window.UpdateStatistics(statistics.moves, activeTarget, activeStars);
    }

    private void UpdateDynamicData(LevelStatistics statistics)
    {
        var viewTime = GetViewTime(statistics.passedTime);
        window.Time.text = _localization.LocalizeFormat("game_time", viewTime);
    }

    private object GetViewTime(TimeSpan time)
    {
        var viewTime = new StringBuilder(8);
        Configure(time.Days);
        Configure(time.Hours);
        Configure(time.Minutes, true);
        Configure(time.Seconds, true);

        void Configure(int value, bool required = false)
        { 
            if (value <= 0 && viewTime.Length <= 0 && required == false) return;
            if (viewTime.Length > 0)
                viewTime.Append(':');
            if (value < 10)
                viewTime.Append('0');
            viewTime.Append(value);
        }

        return viewTime.ToString();
    }

    #region Button event handler

    private void ProceedButtonAction(GameWindowAction action)
    {
        switch (action)
        {
            case GameWindowAction.OnClickCloseYourself:
                _gameDirector.DisposeLevel();
                break;
            case GameWindowAction.OnClickUndo:
                _gameDirector.PopLevel();
                UpdateWindowData();
                break;
            case GameWindowAction.OnClickHint:
                ClickHint();
                break;
            case GameWindowAction.OnClickTeleportation:
                break;
            case GameWindowAction.OnClickContextMenu:
                break;
            case GameWindowAction.OnClickRestart:
                ClickRestart();
                break;
            case GameWindowAction.OnClickGoToMenu:
                ClickGoHome();
                break;
            default:
                Log.WriteError($"Unknown action {action}");
                break;
            
        }
    }

    private void ClickHint()
    {
        const int spendHints = 1;
        if (_playerResourcesDirector.UserData.Hints.CanSpend(spendHints) == false)
        {
            _gameDirector.Pause();
            _windowsDirector.OpenShopWindow();
            return;
        }
        
        _playerResourcesDirector.UpdateData(data =>
        {
            data.Hints.Spend(spendHints);
        });
        
        if(window.Hint.IsActive)
            window.Hint.Disable();
        _levelDirector.ActivateHint();
        
        _signalBus.Fire(new ActivateHint
        {
            levelId = _levelDirector.ActiveLevel.LevelId,
            countActivated = _levelDirector.ActiveLevel.Data.CountActiveHints
        });
    }

    private void ClickRestart()
    {
        _signalBus.Fire(new LevelRestart
        {
            levelId = _levelDirector.ActiveLevel.LevelId,
        });
        
        _gameDirector.RestartLevel();
    }

    private void ClickGoHome()
    {
        _signalBus.Fire(new LevelGoHome
        {
            levelId = _levelDirector.ActiveLevel.LevelId,
        });
        
        _gameDirector.DisposeLevel();
        _windowsDirector.OpenMainWindow();
    }
    
    #endregion

    public void EnableLevelInteraction()
    {
        foreach (var interactionComponent in window.FadedComponents)
            interactionComponent.Enable();
    }

    public void DisableLevelInteraction()
    {
        foreach (var interactionComponent in window.FadedComponents)
            interactionComponent.Disable();
    }
    
    public void EnableVictoryState()
    {
        foreach (var interactionComponent in window.FadedComponents)
            interactionComponent.Disable();
        window.Hint.TurnOff();
    }
    
    public void DisableHint()
    {
        if (window.Hint.IsActive)
            window.Hint.Disable();
    }

    public void EnableHintIfPossible()
    {
        if (window.Hint.IsActive == false)
            window.Hint.Enable();
    }
}
}