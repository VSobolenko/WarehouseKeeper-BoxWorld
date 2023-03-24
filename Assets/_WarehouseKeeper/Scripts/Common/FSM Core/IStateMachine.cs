using System;

namespace WarehouseKeeper.FSMCore
{
internal interface IStateMachine
{
    BaseState ActiveState { get; }
    BaseState PreviousState { get; }
    event Action<BaseState> OnStateChange;
    void SwitchState(BaseState newState);
}
}