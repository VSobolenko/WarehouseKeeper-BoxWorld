using Game.FSMCore.States;

namespace Game.FSMCore.Transitions
{
public abstract class DirectedTransition<TIn, TData, TOut> : Transition<TData>
{
    private readonly State<TIn, TData> _sourceState;
    private readonly State<TData, TOut> _targetState;

    private protected sealed override bool IsDecidedTransient => stateMachine.ActiveState == _sourceState;

    protected DirectedTransition(IStateMachine stateMachine, State<TIn, TData> sourceState,
                                 State<TData, TOut> targetState) : base(stateMachine, targetState)
    {
        _sourceState = sourceState;
        _targetState = targetState;
    }

    internal sealed override void Transit()
    {
        if (stateMachine.ActiveState == _targetState)
            Log.Warning("Identical transition");

        OnTransit();

        var transitionData = _sourceState.ReturnProcessedResult();
        _sourceState.Dispose();
        ActivateTargetState(transitionData);
        stateMachine.ChangeState(_targetState);
    }
}
}