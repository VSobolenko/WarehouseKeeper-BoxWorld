using System;
using Game.FSMCore.States;

namespace Game.FSMCore
{
public interface IStateMachine
{
    IState ActiveState { get; }
    IState PreviousState { get; }
    event Action<IState> OnStateChange;
    protected internal void ChangeState(IState newState);
}
}