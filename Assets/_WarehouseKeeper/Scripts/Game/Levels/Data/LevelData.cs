using System;
using Game.Repositories;

namespace WarehouseKeeper.Levels
{
[Serializable]
public class LevelData : IHasBasicId
{
    public int Id { get; set; }
    public int StarsReceived { get; set; }
    public int CountMoves { get; set; }
    public int BestMoves { get; set; }
    public int CountPushes { get; set; }
    public int BestPushes { get; set; }
    public int CountActiveHints { get; set; }
    public TimeSpan TimeSpent { get; set; }
    public TimeSpan BestTime { get; set; }
}
}