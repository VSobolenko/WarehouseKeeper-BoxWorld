using Game.FSMCore.States;

namespace WarehouseKeeper.Directors.Game.Game_FSM
{
internal class PauseState : State<bool, bool>
{
    protected override bool ReturnProcessedResult() => false;
}
}