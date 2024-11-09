using Game;
using Game.FSMCore;
using Game.FSMCore.Profiler;
using UnityEngine;
using WarehouseKeeper.Directors.Game.Game_FSM.Transitions;
using Zenject;

namespace WarehouseKeeper.Directors.Game.Game_FSM
{
public class GameStateMachineBuilder
{
    private readonly DiContainer _diContainer;
    private readonly FiniteStateMachine _stateMachine;
    public IStateMachine StateMachine => _stateMachine;
    
    private EntryPointState _entryPointState;
    private WaitingState _waitingState;
    private LevelCollectorState _levelCollectorState;
    private PauseState _pauseState;
    private readonly FSMProfilerProvider _fsmProfiler;

    public GameStateMachineBuilder(DiContainer diContainer)
    {
        _diContainer = diContainer;
        _stateMachine = new FiniteStateMachine();
        if (Application.isEditor)
        {
            _fsmProfiler = new GameObject("FSM Linker").AddComponent<FSMProfilerProvider>();
            _fsmProfiler.stateMachine = _stateMachine;
        };

        Log.Info($"Create new {GetType().Name}; Hash={GetHashCode()}");
    }

    public void Initialize()
    {
        InitializeState();
    }

    public void UpdateStateMachine()
    {
        _stateMachine.Update();
    }

    public void ExternalTransit()
    {
        _stateMachine.ForceTransitTo(_waitingState, false);
    }
    
    private void InitializeState()
    {
        var motionState = _diContainer.Instantiate<MotionState>();
        var victoryState = _diContainer.Instantiate<VictoryState>();
        _entryPointState = _diContainer.Instantiate<EntryPointState>();
        _waitingState = _diContainer.Instantiate<WaitingState>();
        _levelCollectorState = _diContainer.Instantiate<LevelCollectorState>();
        _pauseState = _diContainer.Instantiate<PauseState>();

        var waiting2Waiting = _diContainer.Instantiate<Waiting2Waiting>(new object []{_waitingState});
        var entryPoint2Waiting = _diContainer.Instantiate<EntryPoint2Waiting>(new object []{_entryPointState, _waitingState});
        var motion2Victory = _diContainer.Instantiate<Motion2Victory>(new object [] {motionState, victoryState});
        var move2Waiting = _diContainer.Instantiate<Motion2Waiting>(new object [] {motionState, _waitingState});
        var waiting2Move = _diContainer.Instantiate<Waiting2Motion>(new object [] {_waitingState, motionState});
        
        _stateMachine.Tree.AddTransition(entryPoint2Waiting, 3)
                     .AddTransition(motion2Victory, 1)
                     .AddTransition(waiting2Waiting, move2Waiting, waiting2Move)
                     .AddState(_entryPointState, motionState, victoryState, _waitingState, 
                                    _levelCollectorState, _pauseState);
    }

    public void Pause()
    {
        _stateMachine.StopMachine();
        _stateMachine.ForceTransitTo(_pauseState, false);
    }

    public void Resume()
    {
        //_stateMachine.StopMachine();
        _stateMachine.ForceTransitTo(_waitingState, false);
    }
    
    public void StartMachine(int levelId)
    {
        _stateMachine.ForceTransitTo(_entryPointState, levelId);
    }

    public void StopMachine()
    {
        _stateMachine.ForceTransitTo(_levelCollectorState, true);
        _stateMachine.StopMachine();
        if (_fsmProfiler != null)
            Object.Destroy(_fsmProfiler.gameObject);
    }
}
}