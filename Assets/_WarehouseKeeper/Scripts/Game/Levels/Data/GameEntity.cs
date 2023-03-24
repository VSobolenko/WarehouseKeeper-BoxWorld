namespace WarehouseKeeper.Levels
{
internal struct GameEntity
{
    public int X { get; set; }
    public int Y { get; set; }
    public GameEntityType Type { get; set; }
    public EntityGame Entity { get; set; }
}
}