using System;
using Game.FSMCore;
using Game.FSMCore.States;

namespace WarehouseKeeper.Directors.Game.Game_FSM
{
internal class GameStateMachine : IStateMachine
{
    public BaseState ActiveState { get; private set; }
    public BaseState PreviousState { get; private set; }
    
    public event Action<BaseState> OnStateChange;
    public void SwitchState(BaseState newState)
    {
        PreviousState = ActiveState;
        ActiveState = newState;
        OnStateChange?.Invoke(ActiveState);
    }
}
}