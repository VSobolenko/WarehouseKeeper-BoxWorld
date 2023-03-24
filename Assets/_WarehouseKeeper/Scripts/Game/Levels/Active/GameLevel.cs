namespace WarehouseKeeper.Levels
{
internal class GameLevel
{
    public int LevelId { get; }
    public LevelData Data { get; }
    public GameHint Hint { get; }

    public readonly Node[,] nodes;

    public GameEntity[,] entities;

    private int _playerX;
    private int _playerY;
    
    public GameLevel(Node[,] nodes, GameEntity[,] entities, int levelId, LevelData data, LevelSettings hintLevelSettings)
    {
        this.nodes = nodes;
        this.entities = entities;
        LevelId = levelId;
        Data = data;
        Hint = new GameHint(hintLevelSettings);
        
        UpdatePlayerCache();
    }

    public GameEntity GetPlayer() => entities[_playerX, _playerY];

    public void UpdatePlayerCache()
    {
        for (var x = 0; x < entities.GetLength(0); x++)
        {
            for (var y = 0; y < entities.GetLength(1); y++)
            {
                if (entities[x, y].Type != GameEntityType.Player)
                    continue;
                _playerX = x;
                _playerY = y;
                break;
            }
        }
    }
}
}