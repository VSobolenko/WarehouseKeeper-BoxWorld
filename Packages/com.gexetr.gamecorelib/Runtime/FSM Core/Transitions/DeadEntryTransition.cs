using Game.FSMCore.States;

namespace Game.FSMCore.Transitions
{
public abstract class DeadEntryTransition<TIn> : Transition<TIn>
{
    private readonly TIn _inputData;
    private readonly DeadState<TIn> _targetState;

    private protected override bool IsDecidedTransient => stateMachine.ActiveState == null;

    protected DeadEntryTransition(IStateMachine stateMachine, TIn inputData,
                                  DeadState<TIn> targetState) : base(stateMachine, targetState)
    {
        _inputData = inputData;
        _targetState = targetState;
    }

    internal sealed override void Transit()
    {
        if (stateMachine.ActiveState == _targetState)
            Log.Warning("Identical transition");

        OnTransit();
        
        ActivateTargetState(_inputData);
        stateMachine.ChangeState(_targetState);
    }
}
}