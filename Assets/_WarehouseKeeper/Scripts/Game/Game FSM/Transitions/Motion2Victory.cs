using Game.FSMCore;
using Game.FSMCore.Transitions;
using UnityEngine;
using WarehouseKeeper.Levels;

namespace WarehouseKeeper.Directors.Game.Game_FSM.Transitions
{
internal class Motion2Victory : AliveTransition<bool>
{
    private readonly LevelDirector _levelDirector;
    private readonly MotionState _motionState;
    
    public Motion2Victory(IStateMachine stateMachine, MotionState sourceState, VictoryState targetState,
                          LevelDirector levelDirector) :
        base(stateMachine, sourceState, targetState)
    {
        _levelDirector = levelDirector;
        _motionState = sourceState;
    }

    protected override bool CanDecide()
    {
        var motion = _motionState.ExitedState;
        if (motion == false)
            return false;
        var victory = _levelDirector.CheckVictory();

        return victory;
    }
}
}