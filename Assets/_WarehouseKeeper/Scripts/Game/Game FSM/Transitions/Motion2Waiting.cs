using Game.FSMCore;
using Game.FSMCore.Transitions;
using UnityEngine;

namespace WarehouseKeeper.Directors.Game.Game_FSM.Transitions
{
internal class Motion2Waiting : AliveTransition<bool>
{
    private readonly MotionState _motionState;

    public Motion2Waiting(IStateMachine stateMachine, MotionState sourceState, WaitingState targetState) :
        base(stateMachine, sourceState, targetState)
    {
        _motionState = sourceState;
    }

    protected override bool CanDecide() => _motionState.ExitedState;
}
}