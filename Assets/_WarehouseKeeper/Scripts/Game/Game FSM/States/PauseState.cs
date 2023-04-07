using Game.FSMCore.States;

namespace WarehouseKeeper.Directors.Game.Game_FSM
{
internal class PauseState : State<bool, bool>
{
    public override StateType Type => StateType.Pause;
    protected override bool OnStateFinished() => false;
}
}