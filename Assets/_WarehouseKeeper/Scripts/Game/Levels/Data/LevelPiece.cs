using System;

namespace WarehouseKeeper.Levels
{
[Serializable]
internal struct LevelPiece
{
    /// <summary>
    /// Game zone type on the map
    /// </summary>
    public PieceType Type { get; set; }
    
    /// <summary>
    /// Game object type at start
    /// </summary>
    public GameEntityType Start { get; set; }
    
    // /// <summary>
    // /// the purpose of arranging game objects on the level
    // /// </summary>
    // public GameEntityType Target { get; set; }
}

internal enum PieceType : byte
{
    PlayZone, 
    Wall,
    Empty,
    Target,
}

internal enum GameEntityType : byte
{
    None,
    Player, 
    Box,
}
}