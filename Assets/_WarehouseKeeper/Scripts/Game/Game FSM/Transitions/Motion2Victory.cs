using System;
using UnityEngine;
using WarehouseKeeper.FSMCore;
using WarehouseKeeper.Levels;

namespace WarehouseKeeper.Directors.Game.Game_FSM.Transitions
{
internal class Motion2Victory : DirectedTransition<Vector2, bool, bool>
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

    public override bool Decide()
    {
        var decide = IsDecidedTransient;
        if (decide == false)
            return false;

        var motion = _motionState.ExitedState;
        if (motion == false)
            return false;
        var victory = _levelDirector.CheckVictory();

        return decide && motion && victory;
    }
}
}