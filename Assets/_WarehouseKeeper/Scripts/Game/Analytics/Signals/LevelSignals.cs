using System;

namespace WarehouseKeeper.Directors.Game.Analytics.Signals
{
public struct LevelGoHome
{
    public int levelId;
}

public struct LevelRestart
{
    public int levelId;
}

public struct LevelStart
{
    public int levelId;
}

public struct LevelVictory
{
    public int levelId;
    public int starReceived;
    public int countActivatedHint;
    public TimeSpan elapsedTime;
}

public struct ActivateHint
{
    public int levelId;
    public int countActivated;
}
}