using System.Threading.Tasks;
using Game;
using Game.AssetContent;
using Game.Factories;
using Game.Pools;
using UnityEngine;
using Zenject;

namespace WarehouseKeeper.Levels
{
internal class LevelBuilder : IInitializable
{
    private readonly LevelRepositoryDirector _levelRepositoryDirector;
    private readonly IAddressablesManager _addressablesManager;
    private readonly IFactoryGameObjects _factory;
    private readonly IObjectPoolManager _objectPool;

    private const string EntityPlayZone = "WorldEntityPlayZone";
    private const string EntityWall = "WorldEntityWall";
    private const string EntityTarget = "WorldEntityTarget";
    private const string EntityPlayer = "EntityPlayer";
    private const string EntityBox = "EntityBox";

    private GameObject _activeLevelRoot;
    
    public LevelBuilder(LevelRepositoryDirector levelRepositoryDirector, IAddressablesManager addressablesManager,
                        IFactoryGameObjects factory, IObjectPoolManager objectPool)
    {
        _levelRepositoryDirector = levelRepositoryDirector;
        _addressablesManager = addressablesManager;
        _factory = factory;
        _objectPool = objectPool;

        _activeLevelRoot = null;
    }

    #region World

    public async Task<Node[,]> BuildLevel(int levelId)
    {
        if (_activeLevelRoot != null)
        {
            Log.Warning($"Can't build level because there is built. Build skipped");
            return null;
        }

        var levelSettings = _levelRepositoryDirector.GetLevelSetting(levelId);
        if (levelSettings == null)
        {
            Log.Warning($"Can't load level with id={levelId}; Settings not found");
            return null;
        }
        _activeLevelRoot = _factory.InstantiateEmpty($"Level-{levelId}_[{levelSettings.Pieces.GetLength(0)}|{levelSettings.Pieces.GetLength(1)}]");

        var prefabs = await LoadLevelPrefabs();

        return BuildLevel(levelSettings, _activeLevelRoot.transform, prefabs);
    }

    public void DestroyLevel(Node[,] activeLevel)
    {
        if (_activeLevelRoot == null)
        {
            Log.Warning("Can't destroy null level");

            return;
        }
        
        for (var x = 0; x < activeLevel.GetLength(0); x++)
        {
            for (var y = 0; y < activeLevel.GetLength(1); y++)
            {
                if (activeLevel[x, y].Type == PieceType.Empty)
                    continue;
                activeLevel[x, y].Entity.Release();
            }
        }

#if UNITY_EDITOR
        if (Application.isPlaying == false)
        {
            Log.Error("Try delete object in editor mode! Use cancellationToken instead");
            Object.DestroyImmediate(_activeLevelRoot);
        }
        else
        {
            Object.Destroy(_activeLevelRoot);
        }
#else
        Object.Destroy(_activeLevelRoot);
#endif
        _activeLevelRoot = null;
    }

    private async Task<PiecePrefabs> LoadLevelPrefabs()
    {
        var wallPrefab = await _addressablesManager.LoadAssetAsync<GameObject>(EntityWall);
        var playZonePrefab = await _addressablesManager.LoadAssetAsync<GameObject>(EntityPlayZone);
        var targetPrefab = await _addressablesManager.LoadAssetAsync<GameObject>(EntityTarget);
        if (wallPrefab == null || playZonePrefab == null || targetPrefab == null)
        {
            Log.InternalError();
            return default;
        }

        var wallEntity = wallPrefab.GetComponent<EntityPiece>();
        var playZoneEntity = playZonePrefab.GetComponent<EntityPiece>();
        var targetEntity = targetPrefab.GetComponent<EntityPiece>();
        if (wallEntity == null || playZoneEntity == null || targetEntity == null)
        {
            Log.InternalError();
            return default;
        }
        
        return new PiecePrefabs(playZoneEntity, wallEntity, targetEntity);
    }
    
    private Node[,] BuildLevel(LevelSettings settings, Transform root, PiecePrefabs prefabs)
    {
        if (prefabs.HasNull())
        {
            Log.InternalError();
            return null;
        }

        var nodes = new Node[settings.Pieces.GetLength(0), settings.Pieces.GetLength(1)];
        for (var x = 0; x < nodes.GetLength(0); x++)
        {
            for (var y = 0; y < nodes.GetLength(1); y++)
            {
                var pieceType = settings.Pieces[x, y].Type;
                var position = new Vector3(x, 0, y);
                var prefab = prefabs.GetPiecePrefab(pieceType);
                EntityPiece activePiece = null;
                if (prefab != null)
                    activePiece = _objectPool.Get(prefab, position, Quaternion.identity, root);

                nodes[x, y] = new Node {X = x, Y = y, Type = pieceType, Entity = activePiece};
            }
        }

        return nodes;
    }

    #endregion

    #region Game

    public async Task<GameEntity[,]> BuildGameEntity(int levelId)
    {
        if (_activeLevelRoot == null)
        {
            Log.Warning("Can't build game entity in empty level");

            return null;
        }
        var levelSettings = _levelRepositoryDirector.GetLevelSetting(levelId);
        if (levelSettings == null)
        {
            Log.Warning($"Can't load game entity with id={levelId}; Settings not found");
            return null;
        }
        
        var prefabs = await LoadGameEntityPrefabs();

        return BuildGameEntity(levelSettings, _activeLevelRoot.transform, prefabs);
    }

    private async Task<GamePrefabs> LoadGameEntityPrefabs()
    {
        var player = await _addressablesManager.LoadAssetAsync<GameObject>(EntityPlayer);
        var box = await _addressablesManager.LoadAssetAsync<GameObject>(EntityBox);
        if (player == null || box == null)
        {
            Log.InternalError();
            return default;
        }

        var playerEntity = player.GetComponent<EntityGame>();
        var boxEntity = box.GetComponent<EntityGame>();
        if (playerEntity == null || boxEntity == null)
        {
            Log.InternalError();
            return default;
        }
        
        return new GamePrefabs(playerEntity, boxEntity);
    }

    private GameEntity[,] BuildGameEntity(LevelSettings settings, Transform root, GamePrefabs prefabs)
    {
        if (prefabs.HasNull())
        {
            Log.InternalError();
            return null;
        }

        var entities = new GameEntity[settings.Pieces.GetLength(0), settings.Pieces.GetLength(1)];
        
        for (var x = 0; x < entities.GetLength(0); x++)
        {
            for (var y = 0; y < entities.GetLength(1); y++)
            {
                var gameEntity = new GameEntity {X = x, Y = y, Entity = null, Type = settings.Pieces[x, y].Start};
                if (settings.Pieces[x, y].Start != GameEntityType.None)
                {
                    var prefab = prefabs.GetGamePrefab(settings.Pieces[x, y].Start);
                    gameEntity.Entity = _objectPool.Get(prefab, root);
                }

                entities[x, y] = gameEntity;
            }
        }

        return entities;
    }

    public void DestroyGameEntity(GameEntity[,] gameEntities)
    {
        if (_activeLevelRoot == null)
        {
            Log.Warning("Can't destroy game entity in null level");

            return;
        }
        
        foreach (var gameEntity in gameEntities)
        {
            if (gameEntity.Type == GameEntityType.None)
                continue;

            if (gameEntity.Entity == null)
            {
                Log.InternalError();
                continue;
            }
            gameEntity.Entity.Release();
        }
    }

    #endregion

    public void SetupGameEntities(Node[,] nodes, ref GameEntity[,] entities)
    {
        for (var x = 0; x < nodes.GetLength(0); x++)
        {
            for (var y = 0; y < nodes.GetLength(1); y++)
            {
                if (nodes[x, y].Type == PieceType.Empty || nodes[x, y].Entity == null)
                    continue;

                if (entities[x, y].Type == GameEntityType.None || entities[x, y].Entity == null)
                    continue;
                
                var piecePosition = nodes[x, y].Entity.transform.position;
                entities[x, y].Entity.transform.position = new Vector3(piecePosition.x, 0, piecePosition.z);
                entities[x, y].Entity.UpdateState(nodes[x, y]);
            }
        }
    }
    
    private readonly struct PiecePrefabs
    {
        private readonly EntityPiece _playZone;
        private readonly EntityPiece _wall;
        private readonly EntityPiece _target;

        public PiecePrefabs(EntityPiece playZone, EntityPiece wall, EntityPiece target)
        {
            this._playZone = playZone;
            this._wall = wall;
            this._target = target;
        }

        public EntityPiece GetPiecePrefab(PieceType type)
        {
            return type switch
            {
                PieceType.PlayZone => _playZone,
                PieceType.Wall => _wall,
                PieceType.Target => _target,
                PieceType.Empty => null,
                _ => throw new System.ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        public bool HasNull() => _playZone == null || _wall == null || _target == null;
    }
    
    private struct GamePrefabs
    {
        private readonly EntityGame _player;
        private readonly EntityGame _box;

        public GamePrefabs(EntityGame player, EntityGame box)
        {
            this._player = player;
            this._box = box;
        }

        public EntityGame GetGamePrefab(GameEntityType type)
        {
            return type switch
            {
                GameEntityType.Box => _box,
                GameEntityType.Player => _player,
                GameEntityType.None => null,
                _ => throw new System.ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
        
        public bool HasNull() => _player == null || _box == null;
    }
    // public void SaveLevelStage() { }
    //
    // public void UndoLevelStage() { }
    //
    // public void PauseTimer() { }
    //
    // public void ResumeTimer() { }
    public void Initialize()
    {
        // var nodes = await BuildLevel(0);
        // await Task.Delay(1000);
        // var ent = await BuildGameEntity(0);
        // await Task.Delay(1000);
        // SetupGameEntities(nodes, ent);
        //
        // for (int i = 0; i < 10; i++)
        // {
        //     DestroyGameEntity(ent);
        //     DestroyLevel(nodes);
        //     await Task.Delay(500);
        //
        //     nodes = await BuildLevel(0);
        //     ent = await BuildGameEntity(0);
        //     SetupGameEntities(nodes, ent);
        //     await Task.Delay(500);
        // }
        // DestroyGameEntity(ent);
        // DestroyLevel(nodes);
    }
}
}