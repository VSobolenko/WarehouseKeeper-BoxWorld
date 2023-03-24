using UnityEngine;
using WarehouseKeeper.Directors.UI.Windows;
using WarehouseKeeper.FSMCore;
using WarehouseKeeper.Inputs;
using WarehouseKeeper.Levels;
using WarehouseKeeper.UI.Windows.GameWindows;

namespace WarehouseKeeper.Directors.Game.Game_FSM
{
internal class WaitingState: State<bool, Vector2>
{
    private readonly SwipeManager _swipeManager;
    private readonly WindowsDirector _windowsDirector;
    private readonly LevelDirector _levelDirector;
    private GameWindowMediator _cachedWindow;

    private Vector2 _lastSwipeDirection;
    
    public WaitingState(SwipeManager swipeManager, WindowsDirector windowsDirector, LevelDirector levelDirector)
    {
        _swipeManager = swipeManager;
        _windowsDirector = windowsDirector;
        _levelDirector = levelDirector;
    }

    public override StateType Type => StateType.Waiting;

    protected override void OnStateActivated()
    {
        _swipeManager.OnSwipeNormalized += OnUserSwipe;
        SetupWindow();
    }

    protected override void OnStateUpdated()
    {
    }

    protected override Vector2 OnStateFinished()
    {
        if (_levelDirector.ActiveLevel.Hint.InProgress || _levelDirector.ActiveLevel.Hint.IsActive == false || _levelDirector.ActiveLevel.Hint.IsComplete)
            GetWindow()?.DisableHint();
        _levelDirector.DisableHintView();
        _swipeManager.OnSwipeNormalized -= OnUserSwipe;
        return _lastSwipeDirection;
    }

    public override void Dispose()
    {
        _swipeManager.OnSwipeNormalized -= OnUserSwipe;
    }

    private void OnUserSwipe(Vector2 direction)
    {
        if (IsActiveState == false)
            return;

        _lastSwipeDirection = direction;
    }

    private void SetupWindow()
    {
        if (_levelDirector.Statistics.moves > 0)
            GetWindow()?.EnableLevelInteraction();
        else
            GetWindow()?.DisableLevelInteraction();

        if (_levelDirector.ActiveLevel.Hint.InProgress)
        {
            _levelDirector.EnableHitView();
            GetWindow()?.DisableHint();
        }

        if (_levelDirector.ActiveLevel.Hint.LastStateWithHint)
            GetWindow()?.EnableHintIfPossible(); 
        
        if (_levelDirector.ActiveLevel.Hint.IsActive == false && _levelDirector.Statistics.moves == 0)
            GetWindow()?.EnableHintIfPossible(); 
        
        if (_levelDirector.ActiveLevel.Hint.IsComplete && _levelDirector.ActiveLevel.Hint.LastStateWithHint == false)
            GetWindow()?.DisableHint();
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