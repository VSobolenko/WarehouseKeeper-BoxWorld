using Game.FSMCore.States;

namespace Game.FSMCore.Transitions
{
public abstract class DeadEndTransition<TIn, TOut> : Transition<TOut>
{
    private readonly State<TIn, TOut> _sourceState;
    private readonly DeadState<TOut> _targetState;

    private protected override bool IsDecidedTransient => stateMachine.ActiveState == _sourceState;

    protected DeadEndTransition(IStateMachine stateMachine, State<TIn, TOut> sourceState,
                                DeadState<TOut> targetState) : base(stateMachine, targetState)
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