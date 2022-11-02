using System;
using WarehouseKeeper.Repositories;

namespace WarehouseKeeper.Levels
{
public class LevelData : IHasBasicId
{
    public int Id { get; set; }

    public int countStars;
    public TimeSpan timeSpent;
    public int minMoves;
    public int countActiveHints;
}
}