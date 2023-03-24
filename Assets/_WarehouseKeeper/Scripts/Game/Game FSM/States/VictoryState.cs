using System;
using UnityEngine;
using WarehouseKeeper.Directors.Game.Ads;
using WarehouseKeeper.Directors.Game.UserResources;
using WarehouseKeeper.Directors.UI.Windows;
using WarehouseKeeper.FSMCore;
using WarehouseKeeper.Levels;
using WarehouseKeeper.UI.Windows.GameWindows;

namespace WarehouseKeeper.Directors.Game.Game_FSM
{
internal class VictoryState: State<bool, bool>
{
    private readonly LevelRepositoryDirector _levelRepository;
    private readonly PlayerResourcesDirector _playerResourcesDirector;
    private readonly LevelDirector _levelDirector;
    private readonly LevelProgressDirector _progressDirector;
    private readonly WindowsDirector _windowsDirector;
    private readonly AdsDirector _adsDirector;
    private GameWindowMediator _cachedWindow;
    
    public VictoryState(LevelRepositoryDirector levelRepository, 
                        LevelProgressDirector progressDirector,
                        LevelDirector levelDirector, WindowsDirector windowsDirector, PlayerResourcesDirector playerResourcesDirector, AdsDirector adsDirector)
    {
        _levelRepository = levelRepository;
        _levelDirector = levelDirector;
        _windowsDirector = windowsDirector;
        _playerResourcesDirector = playerResourcesDirector;
        _adsDirector = adsDirector;
        _progressDirector = progressDirector;
    }

    public override StateType Type => StateType.Victory;

    protected override void OnStateActivated()
    {
        var activeLevel = SaveActiveProgress();
        GetWindow().EnableVictoryState();
        _windowsDirector.OpenVictoryWindow(activeLevel.Id, _levelDirector.Statistics);
        _adsDirector.TryShowAd();
    }
    
    protected override void OnStateUpdated()
    {
    }

    protected override bool OnStateFinished()
    {
        return false;
    }

    private LevelData SaveActiveProgress()
    {
        var data = CreateOrUpdateNewLevelData();
        var unlockLevelId = data.Id + 1;

        if (_levelRepository.GetLevelData(unlockLevelId) == null)
        {
            _playerResourcesDirector.UpdateData(userData =>
            {
                userData.Amber.Add(15);
            });
        }
        _levelRepository.SaveLevelData(data);

        if (_levelRepository.GetLevelSetting(unlockLevelId) != null &&
            _levelRepository.GetLevelData(unlockLevelId) == null)
            _levelRepository.CreateEmptyData(unlockLevelId);
        return data;
    }

    private LevelData CreateOrUpdateNewLevelData()
    {
        var starReceived = _progressDirector.GetActiveStars();

        return new LevelData
        {
            Id = _levelDirector.ActiveLevel.LevelId,
            StarsReceived = Mathf.Max(starReceived, _levelDirector.ActiveLevel.Data.StarsReceived),
            
            CountMoves = _levelDirector.Statistics.moves + _levelDirector.ActiveLevel.Data.CountMoves,
            BestMoves = Mathf.Min(_levelDirector.Statistics.moves, _levelDirector.ActiveLevel.Data.BestMoves),
            
            CountPushes = _levelDirector.Statistics.pushes + _levelDirector.ActiveLevel.Data.CountPushes,
            BestPushes = Mathf.Min(_levelDirector.Statistics.pushes, _levelDirector.ActiveLevel.Data.BestPushes),
            
            CountActiveHints = _levelDirector.ActiveLevel.Data.CountActiveHints,
            TimeSpent = _levelDirector.ActiveLevel.Data.TimeSpent.Add(_levelDirector.Statistics.passedTime),
            BestTime = _levelDirector.Statistics.passedTime < _levelDirector.ActiveLevel.Data.BestTime
                ? _levelDirector.Statistics.passedTime
                : _levelDirector.ActiveLevel.Data.BestTime
        };
    }
    
    private GameWindowMediator GetWindow()
    {
        if (_cachedWindow != null)
            return _cachedWindow;
        _cachedWindow = _windowsDirector.GetFirstOrDefaultWindow<GameWindowMediator>();

        return _cachedWindow;
    }
}
}