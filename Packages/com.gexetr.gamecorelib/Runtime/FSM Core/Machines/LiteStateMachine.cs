using System;
using System.Collections.Generic;
using Game.FSMCore.States;

namespace Game.FSMCore
{
public interface IStateMachineOperator
{
    void Update();
    void StopMachine();
}

public class LiteStateMachine : IStateMachine, IStateMachineOperator
{
    public IState ActiveState { get; private set; }
    public IState PreviousState { get; private set; }
    public event Action<IState> OnStateChange;

    void IStateMachine.ChangeState(IState newState) => SwitchState(newState);

    private readonly Dictionary<Type, IState> _states = new();

    public void AddState<TState>(TState state) where TState : IState => _states.Add(typeof(TState), state);

    public bool RemoveState<TState>(TState state) where TState : IState => _states.Remove(typeof(TState));

    public void Update() => ActiveState?.UpdateState();

    public void StopMachine()
    {
        ActiveState.Dispose();
        ActiveState = null;
    }

    public void TransitTo<TState, TIn>(TIn data) where TState : DeadState<TIn>
    {
        ActiveState?.Dispose();
        var state = GetState<TState>();
        SwitchState(state);
        Activate(state, data);
    }

    private void SwitchState(IState state)
    {
        PreviousState = ActiveState;
        ActiveState = state;
        OnStateChange?.Invoke(ActiveState);    }

    private TState GetState<TState>() where TState : class, IState
    {
        return _states[typeof(TState)] as TState;
    }

    private void Activate<TIn>(IActivatedState<TIn> state, TIn data)
    {
        state.ActiveState(this, data);
    }
}
}