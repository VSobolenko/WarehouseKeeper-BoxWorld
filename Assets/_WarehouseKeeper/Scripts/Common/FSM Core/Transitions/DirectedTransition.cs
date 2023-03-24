using System;
using WarehouseKeeper.Extension;

namespace WarehouseKeeper.FSMCore
{
internal abstract class DirectedTransition<TSourceInput, TTransitionData, TTargetOutput> : BaseTransition
{
    protected readonly IStateMachine stateMachine;
    private readonly State<TSourceInput, TTransitionData> _sourceState;
    private readonly State<TTransitionData, TTargetOutput> _targetState;

    protected bool IsDecidedTransient => stateMachine.ActiveState.Type == _sourceState.Type;
    
    protected DirectedTransition(IStateMachine stateMachine, State<TSourceInput, TTransitionData> sourceState, State<TTransitionData, TTargetOutput> targetState)
    {
        this.stateMachine = stateMachine;
        _sourceState = sourceState;
        _targetState = targetState;
    }

    public sealed override void Transit()
    {
        if (stateMachine?.ActiveState == _targetState)
            Log.WriteWarning("Identical transition");

        try
        {
            TTransitionData transitionData = default;

            OnTransit();

            if (_sourceState != null)
                transitionData = _sourceState.FinishState();
            
            _targetState?.ActiveState(stateMachine, transitionData);
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogException(e);
#elif DEVELOPMENT_BUILD
            Extension.Log.WriteError($"[OnStateFinished] {e.Message}");
#endif
        }
    }

    protected virtual void OnTransit() { }
}
}