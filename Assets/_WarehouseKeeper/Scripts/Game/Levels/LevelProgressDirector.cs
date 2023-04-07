using Game;
using UnityEngine;

namespace WarehouseKeeper.Levels
{
internal class LevelProgressDirector
{
    private readonly LevelDirector _levelDirector;
    private readonly LevelRepositoryDirector _levelRepository;

    private const int MaxStars = 3;
    private const int DecayPercentage = 30;

    public LevelProgressDirector(LevelDirector levelDirector,
                                 LevelRepositoryDirector levelRepository)
    {
        _levelDirector = levelDirector;
        _levelRepository = levelRepository;
    }

    public int GetActiveStars()
    {
        if (_levelDirector.ActiveLevel == null)
        {
            Log.InternalError();

            return 0;
        }

        var levelSettings = _levelRepository.GetLevelSetting(_levelDirector.ActiveLevel.LevelId);

        return GetActiveStars(levelSettings, _levelDirector.Statistics);
    }
    
    public int GetActiveTarget()
    {
        if (_levelDirector.ActiveLevel == null)
        {
            Log.InternalError();

            return 0;
        }

        var levelSettings = _levelRepository.GetLevelSetting(_levelDirector.ActiveLevel.LevelId);

        return GetMaxStarTarget(levelSettings, _levelDirector.Statistics);
    }
    
    public static int GetActiveStars(LevelSettings levelSettings, LevelStatistics statistics)
    {
        if (levelSettings == null)
        {
            Log.InternalError();

            return 0;
        }
        
        var minimumMoves = levelSettings.Walkthrough.Length;
        var currentMoves = statistics.moves;

        if (currentMoves <= minimumMoves)
            return MaxStars;

        var percentageStep = Mathf.CeilToInt(minimumMoves * (DecayPercentage / 100f));
        if (percentageStep == 0)
            percentageStep = 1;
        
        var overflowSteps = currentMoves - minimumMoves;
        var rollbackStep = (overflowSteps - (overflowSteps % percentageStep)) / percentageStep;
        if (rollbackStep == 0)
            rollbackStep = 1;
        var targetStars = MaxStars - rollbackStep;
        
        return targetStars > 0 ? targetStars : 1;
    }

    public static int GetMaxStarTarget(LevelSettings levelSettings, LevelStatistics statistics)
    {
        if (levelSettings == null)
        {
            Log.InternalError();

            return 0;
        }
        
        var minimumMoves = levelSettings.Walkthrough.Length;
        var currentMoves = statistics.moves;

        if (currentMoves <= minimumMoves)
            return minimumMoves;

        var percentageStep = Mathf.CeilToInt(minimumMoves * (DecayPercentage / 100f));
        if (percentageStep == 0)
            percentageStep = 1;
        var lostStar = (MaxStars - GetActiveStars(levelSettings, statistics)) * 2;

        return minimumMoves + percentageStep * lostStar - 1;
    }
}
}