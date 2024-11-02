using System;

namespace Game.FSMCore.States
{
public interface IState : IDisposable
{
    void UpdateState();
}

internal interface IActivatedState<in TIn> : IState
{
    void ActiveState(IStateMachine machine, TIn data);
}
}