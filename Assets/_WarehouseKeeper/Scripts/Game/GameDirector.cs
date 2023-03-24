using System;
using System.Linq;
using WarehouseKeeper.Directors.Game.Game_FSM;
using WarehouseKeeper.Directors.Game.Game_FSM.Transitions;
using WarehouseKeeper.Directors.UI.Windows;
using WarehouseKeeper.Extension;
using WarehouseKeeper.Levels;
using WarehouseKeeper.UI.Windows.GameWindows;
using Zenject;

namespace WarehouseKeeper.Directors.Game
{
internal class GameDirector : IInitializable, ITickable
{
    private readonly DiContainer _diContainer;
    private readonly WindowsDirector _windowsDirector;
    private readonly LevelDirector _levelDirector;
    private readonly LevelRepositoryDirector _levelRepositoryDirector;

    private GameStateMachineTree _stateMachine;

    public GameDirector(DiContainer diContainer, 
                        WindowsDirector windowsDirector, 
                        LevelDirector levelDirector, LevelRepositoryDirector levelRepositoryDirector)
    {
        _diContainer = diContainer;
        _windowsDirector = windowsDirector;
        _levelDirector = levelDirector;
        _levelRepositoryDirector = levelRepositoryDirector;
    }

    public void Initialize()
    {
        var firstLevelSettings = _levelRepositoryDirector.GetLevelsSettings().FirstOrDefault();
        if (firstLevelSettings == null)
        {
            Log.WriteError("Null levels");
            return;
        }
        var firstLevelData = _levelRepositoryDirector.GetLevelData(firstLevelSettings.Id);
        if (firstLevelData == null)
        {
            _levelRepositoryDirector.CreateEmptyData(firstLevelSettings.Id);
        }
    }

    public void Tick()
    {
        _stateMachine?.UpdateStateMachine();
    }

    public void StartLevel(int id)
    {
        _stateMachine = _diContainer.Resolve<GameStateMachineTree>();
        _stateMachine.Initialize();
        _stateMachine.StartMachine(id);
    }

    public void RestartLevel()
    {
        var activeLevelId = _levelDirector.ActiveLevel.LevelId;
        DisposeLevel();
        StartLevel(activeLevelId);
    }

    public void PopLevel()
    {
        _levelDirector.PopGameStage();
        _stateMachine.ExternalTransit<Waiting2Waiting>();
    }

    public void DisposeLevel()
    {
        _stateMachine.StopMachine();
        _windowsDirector.CloseWindow<GameWindowMediator>();
        _stateMachine = null;
    }

    public void Pause()
    {
        _stateMachine?.Pause();
    }
    
    public void Resume()
    {
        _stateMachine?.Resume();
    }
}
}