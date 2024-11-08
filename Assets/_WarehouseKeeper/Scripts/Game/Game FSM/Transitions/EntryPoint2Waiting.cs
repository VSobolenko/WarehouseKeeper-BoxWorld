using System;
using Game.FSMCore;
using Game.FSMCore.Transitions;
using WarehouseKeeper.Levels;

namespace WarehouseKeeper.Directors.Game.Game_FSM.Transitions
{
internal class EntryPoint2Waiting : AliveTransition<bool>
{
    private readonly LevelDirector _levelDirector;

    public EntryPoint2Waiting(IStateMachine stateMachine, EntryPointState sourceState, WaitingState targetState,
                              LevelDirector levelDirector) : base(stateMachine, sourceState, targetState)
    {
        _levelDirector = levelDirector;
    }

    protected override bool CanDecide() => _levelDirector.ActiveLevel != null;
}
}