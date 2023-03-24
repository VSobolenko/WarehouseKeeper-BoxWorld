using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using WarehouseKeeper.Directors.Game.SceneData;
using WarehouseKeeper.Directors.Game.UserResources;
using WarehouseKeeper.Extension;
using Zenject;

namespace WarehouseKeeper.Levels
{
internal class LevelDirector : ITickable, IDisposable
{
    private readonly LevelRepositoryDirector _levelRepositoryDirector;
    private readonly LevelBuilder _levelBuilder;
    private readonly DiContainer _diContainer;
    private readonly Stack<StageData> _levelStages = new();
    private readonly GameCameraController _gameCamera;

    private LevelStatistics _statistics;
    public GameLevel ActiveLevel { get; private set; }
    public LevelStatistics Statistics => _statistics;
    public event Action<LevelStatistics> OnUpdateStaticData; 
    public event Action<LevelStatistics> OnUpdateDynamicData; 

    public LevelDirector(LevelRepositoryDirector levelRepositoryDirector, LevelBuilder levelBuilder, DiContainer diContainer, PlayerResourcesDirector playerResourcesDirector, GameCameraController gameCamera)
    {
        _levelRepositoryDirector = levelRepositoryDirector;
        _levelBuilder = levelBuilder;
        _diContainer = diContainer;
        _gameCamera = gameCamera;
    }

    public void Tick()
    {
        if (ActiveLevel == null)
            return;
        
        UpdateFrameStatistics();
    }
    
    public void Dispose() { }

    public async Task StartLevel(int levelId)
    {
        var settings = _levelRepositoryDirector.GetLevelSetting(levelId);
        if (settings == null)
        {
            Log.InternalError();
            return;
        }
        _gameCamera.SetupCamera(settings.Pieces.GetLength(0), settings.Pieces.GetLength(1));

        var level = await _levelBuilder.BuildLevel(levelId);
        var gameEntity = await _levelBuilder.BuildGameEntity(levelId);
        var data = _levelRepositoryDirector.GetLevelData(levelId) ?? _levelRepositoryDirector.CreateEmptyData(levelId);

        _levelBuilder.SetupGameEntities(level, ref gameEntity);
        _levelStages.Clear();

        ActiveLevel = new GameLevel(level, gameEntity, levelId, data, settings);
        _statistics = new LevelStatistics();
    }

    public void DestroyLevel()
    {
        if (ActiveLevel == null)
        {
            Log.InternalError();
            return;
        }
        _levelBuilder.DestroyGameEntity(ActiveLevel.entities);
        _levelBuilder.DestroyLevel(ActiveLevel.nodes);
        _levelStages.Clear();
        _levelHintView?.DisposeHint();
        ActiveLevel = null;
    }

    public bool CheckVictory()
    {
        return CheckVictory(ref ActiveLevel.entities, ActiveLevel.nodes);
    }

    private bool CheckVictory(ref GameEntity[,] entities, Node[,] nodes)
    {
        for (int x = 0; x < nodes.GetLength(0); x++)
        {
            for (int y = 0; y < nodes.GetLength(1); y++)
            {
                if (nodes[x, y].Type != PieceType.Target)
                    continue;
                if (entities[x, y].Type != GameEntityType.Box)
                    return false;
            }
        }

        return true;
    }

    public bool Teleport(EntityPiece entityPiece)
    {
        Node node = null;
        var nodeX = 0;
        var nodeY = 0;
        for (int x = 0; x < ActiveLevel.nodes.GetLength(0); x++)
        {
            for (int y = 0; y < ActiveLevel.nodes.GetLength(1); y++)
            {
                if (ActiveLevel.nodes[x, y].Entity == entityPiece)
                {
                    node = ActiveLevel.nodes[x, y];
                    nodeX = x;
                    nodeY = y;
                }
            }
        }

        if (node == null)
        {
            Log.InternalError();
            return false;
        }

        if (ActiveLevel.entities[nodeX, nodeY].Type != GameEntityType.None)
        {
            return false;
        }

        return true;
    }
    
    #region Stage

    public void SaveLevelStage()
    {
        if (ActiveLevel == null)
        {
            Log.InternalError();
            return;
        }
        
        var entityStageCopy = new GameEntity[ActiveLevel.entities.GetLength(0), ActiveLevel.entities.GetLength(1)];
        for (var x = 0; x < ActiveLevel.entities.GetLength(0); x++)
        {
            for (var y = 0; y < ActiveLevel.entities.GetLength(1); y++)
            {
                entityStageCopy[x, y] = ActiveLevel.entities[x, y];
            }
        }
        _levelStages.Push(new StageData
        {
            entities = entityStageCopy,
            statistics = _statistics,
        });
    }

    public void PopGameStage()
    {
        if (_levelStages.TryPop(out var levelStage) == false)
            return;
        
        _levelBuilder.SetupGameEntities(ActiveLevel.nodes, ref levelStage.entities);
        ActiveLevel.entities = levelStage.entities;
        levelStage.statistics.passedTime = _statistics.passedTime;
        _statistics = levelStage.statistics;
        
        ActiveLevel?.UpdatePlayerCache();
        ActiveLevel?.Hint?.DecreaseStep();
        
#if UNITY_EDITOR
        var hintTunel = GameObject.FindObjectOfType<WarehouseKeeper.EditorTools.Levels.HintTunel>();
        if (hintTunel == null)
            Log.WriteWarning("Cannot find hint tunel");
        else
            hintTunel.RemoveLast();
#endif
    }

    private struct StageData
    {
        public GameEntity[,] entities;
        public LevelStatistics statistics;
    }

    #endregion

    #region Statistics

    private void UpdateFrameStatistics()
    {
        var passedTime = Time.deltaTime;
        _statistics.IncreaseTime(passedTime);
        OnUpdateDynamicData?.Invoke(_statistics);
    }

    public void IncreaseMoves()
    {
        _statistics.IncreaseMoves();
        OnUpdateStaticData?.Invoke(_statistics);
    }
    
    public void IncreasePushes()
    {
        _statistics.IncreaseMoves();
        _statistics.IncreasePushes();
        OnUpdateStaticData?.Invoke(_statistics);
    }
    
    #endregion

    #region Hints

    private LevelHint _levelHintView;

    public void ActivateHint()
    {
        if (_levelHintView == null)
            _levelHintView = _diContainer.Resolve<LevelHint>();
        
        var countActivate = ++ActiveLevel.Data.CountActiveHints;
        _levelRepositoryDirector.SaveLevelData(ActiveLevel.Data);
        ActiveLevel.Hint.Activate(countActivate, Statistics.moves);
        EnableHitView();
    }
    
    public void EnableHitView()
    {
        if (_levelHintView == null)
            _levelHintView = _diContainer.Resolve<LevelHint>();
        _levelHintView.EnableView();
    }

    public void DisableHintView()
    {
        if (_levelHintView == null)
            _levelHintView = _diContainer.Resolve<LevelHint>();
        _levelHintView.DisableView();
    }
    
    // public bool HintIsActive => _activeHint != null;
    // public bool AccessToHint => _activeHint == null ? false : _activeHint.IsComplete;
    // //public bool HintCanBeActivated => _activeHint == null;
    // public Vector2 TargetDirection => _activeHint.TargetDirection;
    //
    // private LevelHint _activeHint;
    //
    // public void ActivateHint()
    // {
    //     _activeHint?.FinalizeHint();
    //
    //     _activeHint = _diContainer.Resolve<LevelHint>();
    //     _activeHint.SetCustomStage(Statistics.moves);
    //     _activeHint.EnableView();
    // }
    //
    // public void UpdateHintBeforeMoving()
    // {
    //     if (_activeHint == null)
    //         return;
    //     _activeHint.DisableView();
    //     _activeHint.IncreaseState();
    //     if (_activeHint.IsComplete)
    //     {
    //         _activeHint.FinalizeHint();
    //     }
    // }
    //
    // public void UpdateHintAfterMoving()
    // {
    //     if (_activeHint == null)
    //         return;
    //     if (_activeHint.IsComplete)
    //     {
    //         _activeHint = null;
    //         return;
    //     }
    //     _activeHint.EnableView();
    // }
    //
    // public void EnableHintView()
    // {
    //     _activeHint?.EnableView();
    // }
    //
    // public void DisableHintView()
    // {
    //     _activeHint?.DisableView();
    // }
    //
    // public void UpdateHint()
    // {
    //     _lastMoveIsHint = false;
    //     if (_activeHint == null)
    //         return;
    //
    //     if (_activeHint.HintIsActive)// && _activeHint.IsComplete == false)
    //     {
    //         _activeHint?.UpdateHint();   
    //     }
    //     else
    //     {
    //         DisposeHint();
    //     }
    //
    //     _lastMoveIsHint = true;
    // }

    // private void DisposeHint()
    // {
    //     _activeHint?.FinalizeHint();
    //     _activeHint = null;
    // }

    #endregion
}

internal struct LevelStatistics
{
    public int moves;
    public int pushes;
    public TimeSpan passedTime;

    public void IncreaseTime(float seconds)
    {
        passedTime += TimeSpan.FromSeconds(seconds);
    }

    public void IncreaseMoves()
    {
        moves++;
    }

    public void IncreasePushes()
    {
        pushes++;
    }
}
}