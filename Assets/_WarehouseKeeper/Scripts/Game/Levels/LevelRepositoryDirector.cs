using System;
using System.Collections.Generic;
using System.Linq;
using WarehouseKeeper.Extension;
using WarehouseKeeper.Repositories;

namespace WarehouseKeeper.Levels
{
internal class LevelRepositoryDirector
{
    private readonly IRepository<LevelData> _repositoryData;
    private readonly IRepository<LevelSettings> _repositorySettings;

    private List<LevelData> _cacheLevelsData;
    private List<LevelSettings> _cacheLevelsSettings;
    
    public LevelRepositoryDirector(IRepository<LevelData> repositoryData, IRepository<LevelSettings> repositorySettings)
    {
        _repositoryData = repositoryData;
        _repositorySettings = repositorySettings;

        _cacheLevelsData = _repositoryData.ReadAll().ToList();
    }

    #region Level progress
    
    public LevelData GetLevelData(int levelId) => _repositoryData.Read(levelId);

    public IEnumerable<LevelData> GetLevelsData()
    {
        if (_cacheLevelsData == null)
            _cacheLevelsData = _repositoryData.ReadAll().ToList();

        return _cacheLevelsData;
    }

    public void SaveLevelData(LevelData data)
    {
        if (data == null)
        {
            Log.WriteError("Input data is null. Save skipped");
            return;
        }
        
        var existData = _repositoryData.Read(data.Id);
        if (existData != null)
            _repositoryData.Update(data);
        else
            _repositoryData.CreateById(data);

        _cacheLevelsData ??= _repositoryData.ReadAll().ToList();
        
        var cacheData = _cacheLevelsData.FirstOrDefault(x => x.Id == data.Id);
        if (cacheData == null)
            _cacheLevelsData = _repositoryData.ReadAll().ToList();
        else
            _cacheLevelsData[_cacheLevelsData.IndexOf(cacheData)] = data;
    }
    
    public LevelData CreateEmptyData(int levelId)
    {
        var emptyData = new LevelData()
        {
            Id = levelId,
            StarsReceived = 0,
            CountMoves = 0,
            BestMoves = int.MaxValue,
            CountPushes = 0,
            BestPushes = int.MaxValue,
            CountActiveHints = 0,
            TimeSpent = TimeSpan.Zero,
            BestTime = TimeSpan.MaxValue,
        };
            
        _repositoryData.CreateById(emptyData);

        _cacheLevelsData ??= _repositoryData.ReadAll().ToList();
        
        var cacheData = _cacheLevelsData.FirstOrDefault(x => x.Id == levelId);
        if (cacheData == null)
            _cacheLevelsData = _repositoryData.ReadAll().ToList();
        else
            _cacheLevelsData[_cacheLevelsData.IndexOf(cacheData)] = emptyData;

        return emptyData;
    }

    public int ResetAllLevelData()
    {
        var levelsData = _repositoryData.ReadAll();
        var countDelete = 0;
        foreach (var levelData in levelsData)
        {
            countDelete++;
            _repositoryData.Delete(levelData);
        }

        _cacheLevelsData = null;

        return countDelete;
    }

    #endregion

    #region Level settings
    
    public LevelSettings GetLevelSetting(int levelId) => _repositorySettings.Read(levelId);
    
    public IEnumerable<LevelSettings> GetLevelsSettings()
    {
        if (_cacheLevelsSettings == null)
            _cacheLevelsSettings = _repositorySettings.ReadAll().ToList();

        return _cacheLevelsSettings;
    }
    
    #endregion

}
}