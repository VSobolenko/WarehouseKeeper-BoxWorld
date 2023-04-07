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

    public override StateType Type => StateType.None;

    protected override void OnStateActivated()
    {
        _audioDirector.StopGameBackground();
        _levelDirector.DestroyLevel();
    }
    
    protected override void OnStateUpdated()
    {
    }

    protected override bool OnStateFinished()
    {
        return false;
    }
}
}