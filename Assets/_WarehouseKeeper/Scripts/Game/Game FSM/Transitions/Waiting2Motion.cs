using UnityEngine;
using WarehouseKeeper.Directors.UI.Windows;
using WarehouseKeeper.FSMCore;
using WarehouseKeeper.Inputs;
using WarehouseKeeper.Levels;
using WarehouseKeeper.UI.Windows.GameWindows;

namespace WarehouseKeeper.Directors.Game.Game_FSM.Transitions
{
internal class Waiting2Motion : DirectedTransition<bool, Vector2, bool>
{
    private readonly SwipeManager _swipeManager;
    private readonly WaitingState _waitingState;
    private readonly MotionState _motionState;
    private readonly LevelDirector _levelDirector;
    private readonly WindowsDirector _windowsDirector;
    private GameWindowMediator _cachedWindow;

    private bool _canDecide = false;
    
    public Waiting2Motion(IStateMachine stateMachine, WaitingState sourceState, MotionState targetState,
                          SwipeManager swipeManager, LevelDirector levelDirector, WindowsDirector windowsDirector) : base(stateMachine, sourceState, targetState)
    {
        _swipeManager = swipeManager;
        _levelDirector = levelDirector;
        _windowsDirector = windowsDirector;
        _waitingState = sourceState;
        _motionState = targetState;
        _swipeManager.OnSwipeNormalized += OnUserSwipe;
    }

    private void OnUserSwipe(Vector2 direction)
    {
        if (IsDecidedTransient == false)
            return;

        _canDecide = true;
    }

    public override bool Decide() => IsDecidedTransient && _canDecide;

    protected override void OnTransit()
    {
        // if (_levelDirector.ActiveLevel.Hint.IsActive == false)
        //     GetWindow()?.DisableHint();

        _canDecide = false;
    }

    public override void Dispose()
    {
        _swipeManager.OnSwipeNormalized -= OnUserSwipe;
    }
    
    private GameWindowMediator GetWindow()
    {
        if (_cachedWindow != null)
            return _cachedWindow;
        _cachedWindow = _windowsDirector.GetFirstOrDefaultWindow<GameWindowMediator>();

        return _cachedWindow;
    }
}
}