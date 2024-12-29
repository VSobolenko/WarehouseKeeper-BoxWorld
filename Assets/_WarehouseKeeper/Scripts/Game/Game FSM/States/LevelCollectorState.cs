using Game.FSMCore.States;
using WarehouseKeeper.Directors.Game.Audio;
using WarehouseKeeper.Levels;

namespace WarehouseKeeper.Directors.Game.Game_FSM
{
internal class LevelCollectorState : State<bool, bool>
{
    private readonly LevelDirector _levelDirector;
    private readonly AudioDirector _audioDirector;

    public LevelCollectorState(LevelDirector levelDirector,
                               AudioDirector audioDirector)
    {
        _levelDirector = levelDirector;
        _audioDirector = audioDirector;
    }

    protected override void OnStateActivated()
    {
        _audioDirector.StopGameBackground();
        _levelDirector.DestroyLevel();
    }

    public override void UpdateState()
    {
    }

    protected override bool ReturnStateProcessedResult()
    {
        return false;
    }
}
}