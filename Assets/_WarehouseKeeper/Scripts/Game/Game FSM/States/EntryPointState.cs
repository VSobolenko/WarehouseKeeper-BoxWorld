using WarehouseKeeper.Directors.Game.Audio;
using WarehouseKeeper.Directors.Game.SceneData;
using WarehouseKeeper.Directors.UI.Windows;
using WarehouseKeeper.FSMCore;
using WarehouseKeeper.Levels;

namespace WarehouseKeeper.Directors.Game.Game_FSM
{
internal class EntryPointState : State<int, bool>
{
    private readonly LevelDirector _levelDirector;
    private readonly WindowsDirector _windowsDirector;
    private readonly AudioDirector _audioDirector;

    public EntryPointState(LevelDirector levelDirector,
                           WindowsDirector windowsDirector, 
                           AudioDirector audioDirector)
    {
        _levelDirector = levelDirector;
        _windowsDirector = windowsDirector;
        _audioDirector = audioDirector;
    }

    public override StateType Type => StateType.EntryPoint;

    protected override async void OnStateActivated()
    {
        _audioDirector.PlayGameBackground();
        await _levelDirector.StartLevel(inputData);
        _windowsDirector.OpenGameWindow(mediator =>
        {
            mediator.UpdateWindowData();
        });
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