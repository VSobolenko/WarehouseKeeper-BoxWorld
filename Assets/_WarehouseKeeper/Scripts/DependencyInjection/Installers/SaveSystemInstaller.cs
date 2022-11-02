using UnityEngine;
using WarehouseKeeper.Levels;
using WarehouseKeeper.Managers.Repositories;
using WarehouseKeeper.Repositories;
using Zenject;

namespace WarehouseKeeper.DependencyInjection.Installers
{
internal class SaveSystemInstaller : Installer<SaveSystemInstaller>
{
#if UNITY_EDITOR
    private static readonly string LevelsDirectory = Application.dataPath + "/UserData/Levels/";
#elif UNITY_IOS
    private static readonly string LevelsDirectory = Application.dataPath + "/UserData/Levels/";
#elif UNITY_ANDROID
    private static readonly string LevelsDirectory = Application.persistentDataPath + "/UserData/Levels/";
#elif UNITY_STANDALONE_OSX
    private static readonly string LevelsDirectory = Application.dataPath + "/UserData/Levels/";
#else
    private static readonly string LevelsDirectory = Application.dataPath + "/UserData/Levels/";
#endif
    
    public override void InstallBindings()
    {
        Container.Bind<IRepository<LevelData>>().To<FileSaveManager<LevelData>>().AsSingle();
        Container.Bind<string>().FromInstance(LevelsDirectory).WhenInjectedInto<IRepository<LevelData>>();
    }
}
}