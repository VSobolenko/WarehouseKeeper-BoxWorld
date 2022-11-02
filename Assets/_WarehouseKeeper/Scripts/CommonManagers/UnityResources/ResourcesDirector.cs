using System;
using UnityEngine;
using WarehouseKeeper._WarehouseKeeper.Scripts.Levels;

namespace WarehouseKeeper.UnityResources.Managers
{
internal class ResourcesDirector
{
    public LevelConfig[] LevelConfigs { get; private set; }
    public ResourcesDirector()
    {
        LoadResources();
    }

    private void LoadResources()
    {
        LevelConfigs = LoadLevelConfigs();
    }

    private LevelConfig[] LoadLevelConfigs()
    {
        throw new NotSupportedException("Create in resources not supported");

        //LevelConfigs = Resources.LoadAll<LevelConfig>("UserData");

    }
}
}