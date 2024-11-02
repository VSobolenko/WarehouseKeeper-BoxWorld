using Game.FSMCore.States;

namespace Game.FSMCore.Transitions
{
public abstract class CircularTransition<TIn, TOut> : Transition<TIn>
{
    private readonly State<TIn, TOut> _state;

    private protected override bool IsDecidedTransient => stateMachine.ActiveState == _state;

    protected CircularTransition(IStateMachine stateMachine, State<TIn, TOut> state) : base(stateMachine, state)
    {
        _state = state;
    }

    internal sealed override void Transit()
    {
        OnTransit();

        _state.ReturnProcessedResult();
        _state.Dispose();
        ActivateTargetState(default);
        stateMachine.ChangeState(_state);
    }
}
}