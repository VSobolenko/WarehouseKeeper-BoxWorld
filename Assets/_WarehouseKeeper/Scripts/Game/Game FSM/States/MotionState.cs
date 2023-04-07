using System.Threading;
using System.Threading.Tasks;
using Game;
using Game.FSMCore.States;
using UnityEngine;
using WarehouseKeeper.Directors.UI.Windows;
using WarehouseKeeper.Levels;
using WarehouseKeeper.UI.Windows.GameWindows;

namespace WarehouseKeeper.Directors.Game.Game_FSM
{
internal class MotionState : State<Vector2, bool>
{
    private readonly LevelDirector _levelDirector;
    private readonly WindowsDirector _windowsDirector;
    private GameWindowMediator _cachedWindow;

    private CancellationTokenSource _tokenSource = new CancellationTokenSource();
    
    public override StateType Type => StateType.Motion;

    public bool ExitedState { get; private set; }

    public MotionState(LevelDirector levelDirector, WindowsDirector windowsDirector)
    {
        _levelDirector = levelDirector;
        _windowsDirector = windowsDirector;

#if UNITY_EDITOR
        var hintTunel = GameObject.FindObjectOfType<WarehouseKeeper.EditorTools.Levels.HintTunel>();
        if (hintTunel == null)
            Log.WriteWarning("Cannot find hint tunel");
        else
            hintTunel.ClearHints();
#endif
    }

    protected override void OnStateActivated()
    {
        ExitedState = false;
        MovePerson(inputData);
        GetWindow()?.DisableLevelInteraction();
    }
    
    protected override void OnStateUpdated()
    {
    }

    protected override bool OnStateFinished()
    {
        return false;
    }

    public override void Dispose()
    {
        _tokenSource.Cancel();
        _tokenSource.Dispose();
    }

    private async void MovePerson(Vector2 direction)
    {
        if (_levelDirector?.ActiveLevel == null)
        {
            Log.InternalError();
            return;
        }

        if (_levelDirector.ActiveLevel.Hint.InProgress && _levelDirector.ActiveLevel.Hint.GetActiveDirection() != direction)
        {
            ExitedState = true;
            return;
        }

        var level = _levelDirector.ActiveLevel;
        var intDirection = new Vector2Int((int) direction.x, (int) direction.y);
        if (CanMoveEntityToPiece(level.nodes, level.GetPlayer().X, level.GetPlayer().Y, intDirection))
        {
            var currentBoxPos = new Vector2Int(level.GetPlayer().X + intDirection.x, level.GetPlayer().Y + intDirection.y);
            if (StayOnWayGameEntity(ref level.entities, level.GetPlayer().X, level.GetPlayer().Y, intDirection,
                                    GameEntityType.Box))
            {
                if (CanMoveEntityToPiece(level.nodes, currentBoxPos.x, currentBoxPos.y, intDirection) &&
                    StayOnWayGameEntity(ref level.entities, currentBoxPos.x, currentBoxPos.y, intDirection, 
                                        GameEntityType.None))
                {
                    //Start Level update
                    _levelDirector.SaveLevelStage();
                    _levelDirector.IncreasePushes();
                    _levelDirector.ActiveLevel.Hint.IncreaseStep();

                    //View moving
                    var entityTask = MoveEntityView(level.nodes, level.entities[currentBoxPos.x, currentBoxPos.y], intDirection, level.entities);
                    var playerTask = MoveEntityView(level.nodes, level.GetPlayer(), intDirection, level.entities);
                    await Task.WhenAll(entityTask, playerTask);
                    if (_tokenSource.IsCancellationRequested)
                        return;
                    
                    //Logic moving
                    MoveEntityLogic(ref level.entities, level.entities[currentBoxPos.x, currentBoxPos.y], intDirection);
                    MoveEntityLogic(ref level.entities, level.GetPlayer(), intDirection);

                    //Finish Level update
                    level.UpdatePlayerCache();
#if UNITY_EDITOR
                    var hintTunel = GameObject.FindObjectOfType<WarehouseKeeper.EditorTools.Levels.HintTunel>();
                    if (hintTunel == null)
                        Log.WriteWarning("Cannot find hint tunel");
                    else
                        hintTunel.AddDirection(new Vector2Int((int) direction.x, (int) direction.y));
#endif
                }
            }
            else
            {
                //Start Level update
                _levelDirector.SaveLevelStage();
                _levelDirector.IncreaseMoves();
                _levelDirector.ActiveLevel.Hint.IncreaseStep();
                
                //View moving
                var playerTask = MoveEntityView(level.nodes, level.GetPlayer(), intDirection, level.entities);
                
                await Task.WhenAll(playerTask);
                if (_tokenSource.IsCancellationRequested)
                    return;
                
                //Logic moving
                MoveEntityLogic(ref level.entities, level.GetPlayer(), intDirection);
                
                //Finish Level update
                level.UpdatePlayerCache();
#if UNITY_EDITOR
                var hintTunel = GameObject.FindObjectOfType<WarehouseKeeper.EditorTools.Levels.HintTunel>();
                if (hintTunel == null)
                    Log.WriteWarning("Cannot find hint tunel");
                else
                    hintTunel.AddDirection(new Vector2Int((int) direction.x, (int) direction.y));
#endif
            }
        }
        ExitedState = true;
    }
    
    private bool CanMoveEntityToPiece(Node[,] nodes, int activeX, int activeY, Vector2Int direction)
    {
        // Конец уровня
        if (IsPointInGameGrid(nodes, activeX + direction.x, activeY + direction.y) == false)
            return false;
        
        // Исключаем невозможные
        if (nodes[activeX + direction.x, activeY + direction.y].Type == PieceType.Empty || 
            nodes[activeX + direction.x, activeY + direction.y].Type == PieceType.Wall)
            return false;

        return true;
    }
    
    private bool StayOnWayGameEntity(ref GameEntity[,] nodes, int activeX, int activeY, Vector2Int direction, params GameEntityType[] types)
    {
        if (IsPointInGameGrid(ref nodes, activeX + direction.x, activeY + direction.y) == false)
            return false;

        foreach (var type in types)
        {
            if (nodes[activeX + direction.x, activeY + direction.y].Type != type)
                continue;

            return true;
        }

        return false;
    }
    
    //ToDo: LevelDirector responsible to switching Entities
    private void MoveEntityLogic(ref GameEntity[,] entities, GameEntity gameEntity, Vector2Int direction)
    {
        //Switch nodes
        var targetEntity = entities[gameEntity.X + direction.x, gameEntity.Y + direction.y];
        entities[targetEntity.X, targetEntity.Y] = gameEntity;
        entities[gameEntity.X, gameEntity.Y] = targetEntity;
        
        //Switch X and Y
        entities[targetEntity.X, targetEntity.Y].X += direction.x;
        entities[targetEntity.X, targetEntity.Y].Y += direction.y;
        
        entities[gameEntity.X, gameEntity.Y].X -= direction.x;
        entities[gameEntity.X, gameEntity.Y].Y -= direction.y;
    }
    
    private async Task MoveEntityView(Node[,] nodes, GameEntity gameEntity, Vector2Int direction, GameEntity[,] entities)
    {
        if (gameEntity.Entity == null)
        {
            Log.InternalError();
            return;
        }
        
        gameEntity.Entity.OnStartMoving(direction, gameEntity, nodes, entities);
        var targetPosition = nodes[gameEntity.X + direction.x, gameEntity.Y + direction.y] 
                             .Entity.transform.position;
        const int moveDuration = 150;
        gameEntity.Entity.StartViewMoving(targetPosition, moveDuration / 1000f, _tokenSource.Token);
        
        await Task.Delay(moveDuration);
        if (_tokenSource.IsCancellationRequested)
            return;
        gameEntity.Entity.StopViewMoving(targetPosition);
        gameEntity.Entity.OnEndMoving();
    }
    
    private bool IsPointInGameGrid(Node[,] nodes, int expectedX, int expectedY) =>
        expectedX < nodes.GetLength(0) && expectedX >= 0 && expectedY < nodes.GetLength(1) && expectedY >= 0;
    
    private bool IsPointInGameGrid(ref GameEntity[,] nodes, int expectedX, int expectedY) =>
        expectedX < nodes.GetLength(0) && expectedX >= 0 && expectedY < nodes.GetLength(1) && expectedY >= 0;
    
    private GameWindowMediator GetWindow()
    {
        if (_cachedWindow != null)
            return _cachedWindow;
        _cachedWindow = _windowsDirector.GetFirstOrDefaultWindow<GameWindowMediator>();

        return _cachedWindow;
    }
}
}