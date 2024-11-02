using System;
using Game.FSMCore.States;

namespace Game.FSMCore
{
public class FiniteStateMachine : IStateMachine, IStateMachineOperator
{
    public IState ActiveState { get; private set; }
    public IState PreviousState { get; private set; }
    public event Action<IState> OnStateChange;
    public StateMachineTree Tree { get; set; } = new();

    void IStateMachine.ChangeState(IState state) => SwitchState(state);

    public void Update()
    {
        ActiveState.UpdateState();
        Tree.UpdateTree();
    }

    public void StopMachine()
    {
        ActiveState?.Dispose();
        SwitchState(null);
        Tree.DisposeMachine();
    }

    public void ForceTransitTo<TIn>(DeadState<TIn> state, TIn data)
    {
        if (Tree.GetStates().Contains(state) == false)
            throw new ArgumentException("Unknown state");

        ActiveState?.Dispose();
        SwitchState(state);
        Activate(state, data);
    }
    
    private void SwitchState(IState state)
    {
        PreviousState = ActiveState;
        ActiveState = state;
        OnStateChange?.Invoke(ActiveState);
    }

    private void Activate<TIn>(IActivatedState<TIn> state, TIn data)
    {
        state.ActiveState(this, data);
    }
}
}