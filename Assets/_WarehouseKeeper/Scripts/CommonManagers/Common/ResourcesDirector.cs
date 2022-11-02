using UnityEngine;
using WarehouseKeeper._WarehouseKeeper.Scripts.Levels;

namespace WarehouseKeeper._WarehouseKeeper.Scripts.CommonManagers.Common
{
public class ResourcesDirector
{
    public LevelConfig[] LevelConfigs { get; private set; }
    public ResourcesDirector()
    {
        LoadResources();
    }

    private void LoadResources()
    {
        var txt = Resources.Load<TextAsset>("");
    }
}
}