using Game.FSMCore;
using Game.FSMCore.Transitions;
using UnityEngine;

namespace WarehouseKeeper.Directors.Game.Game_FSM.Transitions
{
internal class Motion2Waiting : DirectedTransition<Vector2, bool, Vector2>
{
    private readonly MotionState _motionState;

    public Motion2Waiting(IStateMachine stateMachine, MotionState sourceState, WaitingState targetState) :
        base(stateMachine, sourceState, targetState)
    {
        _motionState = sourceState;
    }

    public override bool Decide() => IsDecidedTransient && _motionState.ExitedState;
}
}