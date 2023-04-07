using Game.FSMCore;
using Game.FSMCore.States;
using Game.FSMCore.Transitions;
using UnityEngine;

namespace WarehouseKeeper.Directors.Game.Game_FSM.Transitions
{
internal class Waiting2Waiting : CircularTransition<bool, Vector2>
{
    public Waiting2Waiting(IStateMachine stateMachine, State<bool, Vector2> state) : base(stateMachine, state)
    {
    }

    public override bool Decide() => false;
}
}