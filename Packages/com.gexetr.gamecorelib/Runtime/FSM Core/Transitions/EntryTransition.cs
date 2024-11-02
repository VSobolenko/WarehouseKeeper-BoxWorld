using Game.FSMCore.States;

namespace Game.FSMCore.Transitions
{
public abstract class EntryTransition<TIn, TOut> : Transition<TIn>
{
    private readonly TIn _inputData;
    private readonly State<TIn, TOut> _targetState;

    private protected override bool IsDecidedTransient => stateMachine.ActiveState == null;

    protected EntryTransition(IStateMachine stateMachine, TIn inputData,
                              State<TIn, TOut> targetState) : base(stateMachine, targetState)
    {
        _inputData = inputData;
        _targetState = targetState;
    }

    internal sealed override void Transit()
    {
        if (stateMachine.ActiveState == _targetState)
            Log.Warning("Identical transition");

        OnTransit();

        stateMachine.ChangeState(_targetState);
    }
}
}