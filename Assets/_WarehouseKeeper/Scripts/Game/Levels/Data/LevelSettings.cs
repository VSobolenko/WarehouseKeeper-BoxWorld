using System;
using Game.Repositories;
using UnityEngine;

namespace WarehouseKeeper.Levels
{
[Serializable]
internal class LevelSettings : IHasBasicId
{
    public int Id { get; set; }

    public LevelPiece[,] Pieces { get; set; }
    
    public Vector2IntSerializable[] Walkthrough { get; set; }
    
    public int SpentHints { get; set; }
}

[Serializable]
internal struct Vector2IntSerializable
{
    public int x;
    public int y;

    public Vector2IntSerializable(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Vector2 GetVector2() => new Vector2(x, y);
    public Vector2Int GetVector2Int() => new Vector2Int(x, y);
    public static Vector2IntSerializable Convert(Vector2Int vector) => new Vector2IntSerializable(vector.x, vector.y);
}
}