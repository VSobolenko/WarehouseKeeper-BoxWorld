using System.Collections.Generic;
using System.Linq;
using Game;
using Game.FSMCore;
using Game.FSMCore.States;
using Game.FSMCore.Transitions;
using WarehouseKeeper.Directors.Game.Game_FSM.Transitions;
using Zenject;

namespace WarehouseKeeper.Directors.Game.Game_FSM
{
internal sealed class GameStateMachineTree
{
    private readonly DiContainer _diContainer;
    private readonly GameStateMachine _stateMachine;
    
    private EntryPointState _entryPointState;
    private MotionState _motionState;
    private VictoryState _victoryState;
    private WaitingState _waitingState;
    private LevelCollectorState _levelCollectorState;
    private PauseState _pauseState;

    private readonly List<BaseState> _states = new();
    private readonly StateMachineTree _tree = new();
    
    public GameStateMachineTree(DiContainer diContainer, GameStateMachine stateMachine)
    {
        _diContainer = diContainer;
        _stateMachine = stateMachine;
        Log.WriteInfo($"Create new {GetType().Name}; Hash={GetHashCode()}");
    }

    public void Initialize()
    {
        InitializeStates();
        InitializeTransitions();
    }

    public void UpdateStateMachine()
    {
        _tree.UpdateTree(_stateMachine);
        _stateMachine.ActiveState.UpdateState();
    }

    public void ExternalTransit<TTransition>() where TTransition : BaseTransition
    {
        _tree.GetTransition<TTransition>()?.Transit();
    }
    
    private void InitializeStates()
    {
        _entryPointState = _diContainer.Instantiate<EntryPointState>();
        _motionState = _diContainer.Instantiate<MotionState>();
        _victoryState = _diContainer.Instantiate<VictoryState>();
        _waitingState = _diContainer.Instantiate<WaitingState>();
        _levelCollectorState = _diContainer.Instantiate<LevelCollectorState>();
        _pauseState = _diContainer.Instantiate<PauseState>();
        
        _states.Add(_entryPointState);
        _states.Add(_motionState);
        _states.Add(_victoryState);
        _states.Add(_waitingState);
        _states.Add(_levelCollectorState);
        _states.Add(_pauseState);
    }

    private void InitializeTransitions()
    {
        var waiting2Waiting = _diContainer.Instantiate<Waiting2Waiting>(new object []{_waitingState});
        var entryPoint2Waiting = _diContainer.Instantiate<EntryPoint2Waiting>(new object []{_entryPointState, _waitingState});
        var motion2Victory = _diContainer.Instantiate<Motion2Victory>(new object [] {_motionState, _victoryState});
        var move2Waiting = _diContainer.Instantiate<Motion2Waiting>(new object [] {_motionState, _waitingState});
        var waiting2Move = _diContainer.Instantiate<Waiting2Motion>(new object [] {_waitingState, _motionState});
        
        _tree.AddTransition(entryPoint2Waiting, 3);
        _tree.AddTransition(motion2Victory, 1);
        _tree.AddTransition(waiting2Waiting, move2Waiting, waiting2Move);
    }

    public void Pause()
    {
        foreach (var state in _states.Where(state => state.IsActiveState))
            state.ForceFinish();
        _pauseState.ActiveState(_stateMachine, false);
    }

    public void Resume()
    {
        foreach (var state in _states.Where(state => state.IsActiveState))
            state.ForceFinish();
        _waitingState.ActiveState(_stateMachine, false);
    }
    
    public void StartMachine(int levelId)
    {
        _entryPointState.ActiveState(_stateMachine, levelId);
    }

    public void StopMachine()
    {
        _levelCollectorState.ActiveState(_stateMachine, true);
        foreach (var state in _states)
        {
            state.Dispose();
        }
        _tree.DisposeMachine();
    }
}
}