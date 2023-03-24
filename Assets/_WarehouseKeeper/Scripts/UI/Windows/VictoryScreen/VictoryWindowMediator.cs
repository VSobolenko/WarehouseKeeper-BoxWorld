using System;
using Cysharp.Threading.Tasks;
using WarehouseKeeper.Directors.Game;
using WarehouseKeeper.Directors.Game.Analytics.Signals;
using WarehouseKeeper.Directors.UI.Windows.VictoryScreen.Components;
using WarehouseKeeper.Extension;
using WarehouseKeeper.Gui.Windows.Mediators;
using WarehouseKeeper.Levels;
using WarehouseKeeper.Localizations;
using Zenject;

namespace WarehouseKeeper.Directors.UI.Windows.VictoryScreen
{
internal class VictoryWindowMediator : BaseMediator<VictoryWindowView>
{
    private readonly LevelRepositoryDirector _levelRepositoryDirector;
    private readonly GameDirector _gameDirector;
    private readonly WindowsDirector _windowsDirector;
    private readonly ILocalizationManager _localizationManager;
    private readonly SignalBus _signalBus;

    private int _viewLevelId;
    
    public VictoryWindowMediator(VictoryWindowView window, LevelRepositoryDirector levelRepositoryDirector, GameDirector gameDirector, WindowsDirector windowsDirector, ILocalizationManager localizationManager, SignalBus signalBus) : base(window)
    {
        _levelRepositoryDirector = levelRepositoryDirector;
        _gameDirector = gameDirector;
        _windowsDirector = windowsDirector;
        _localizationManager = localizationManager;
        _signalBus = signalBus;
    }
    
    public override void OnInitialize()
    {
        window.OnWindowAction += ProceedButtonAction;
    }

    public override void OnDestroy()
    {
        window.OnWindowAction -= ProceedButtonAction;
    }

    public void Setup(int levelId, LevelStatistics levelStatistics)
    {
        _viewLevelId = levelId;
        var data = _levelRepositoryDirector.GetLevelData(levelId);
        var settings = _levelRepositoryDirector.GetLevelSetting(levelId);
        var nextData = _levelRepositoryDirector.GetLevelData(levelId + 1);
        window.SetActiveNextLevel(nextData != null);
        window.Setup(data, levelStatistics, _localizationManager);

        SendEventVictoryOnNextFrame(levelId, data.StarsReceived, levelStatistics.passedTime, data.CountActiveHints);
    }
    
    #region Button event handler

    private void ProceedButtonAction(VictoryWindowAction action)
    {
        switch (action)
        {
            case VictoryWindowAction.OnClickGoToMenu:
                _gameDirector.DisposeLevel();
                _windowsDirector.CloseWindow(this);
                _windowsDirector.OpenMainWindow();
                break;
            case VictoryWindowAction.OnClickRestart:
                _windowsDirector.CloseWindow(this);
                _gameDirector.RestartLevel();
                break;
            case VictoryWindowAction.OnClickNextLevel:
                var data = _levelRepositoryDirector.GetLevelData(_viewLevelId + 1);
                if (data == null)
                {
                    Log.InternalError();
                    return;
                }
                
                _gameDirector.DisposeLevel();
                _windowsDirector.CloseWindow(this);
                _gameDirector.StartLevel(data.Id);
                break;
            default:
                Log.WriteError($"Unknown action {action}");
                break;
        }
    }

    #endregion

    private async void SendEventVictoryOnNextFrame(int levelId, int starReceived, TimeSpan elapsedTime,
                                                   int countActivatedHint)
    {
        await UniTask.DelayFrame(2);
        
        _signalBus.Fire(new LevelVictory
        {
            levelId = levelId,
            starReceived = starReceived,
            countActivatedHint = countActivatedHint,
            elapsedTime = elapsedTime,
        });
    }
}
}