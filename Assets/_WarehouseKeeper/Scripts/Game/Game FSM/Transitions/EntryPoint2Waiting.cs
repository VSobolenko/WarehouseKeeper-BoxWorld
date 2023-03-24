using System;
using UnityEngine;
using WarehouseKeeper.FSMCore;
using WarehouseKeeper.Levels;

namespace WarehouseKeeper.Directors.Game.Game_FSM.Transitions
{
internal class EntryPoint2Waiting : DirectedTransition<int, bool, Vector2>
{
    private readonly LevelDirector _levelDirector;

    public EntryPoint2Waiting(IStateMachine stateMachine, EntryPointState sourceState, WaitingState targetState,
                              LevelDirector levelDirector) : base(stateMachine, sourceState, targetState)
    {
        _levelDirector = levelDirector;
    }

    public override bool Decide() => IsDecidedTransient && _levelDirector.ActiveLevel != null;
}
}