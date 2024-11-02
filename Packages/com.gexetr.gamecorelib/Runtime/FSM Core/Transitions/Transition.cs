using Game.FSMCore.States;

namespace Game.FSMCore.Transitions
{
public abstract class Transition<TIn> : BaseTransition
{
    //add access to protected or only internal?
    protected internal readonly IStateMachine stateMachine;
    private readonly IActivatedState<TIn> _state;

    internal Transition(IStateMachine stateMachine, IActivatedState<TIn> state)
    {
        this.stateMachine = stateMachine;
        _state = state;
    }

    private protected abstract bool IsDecidedTransient { get; }

    protected void ActivateTargetState(TIn data)
    {
        _state.ActiveState(stateMachine, data);
    }

    internal sealed override bool Decide() => IsDecidedTransient && CanDecide();

    protected abstract bool CanDecide();
    protected virtual void OnTransit() { }
}
}