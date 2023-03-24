using UnityEngine;
using WarehouseKeeper.Directors.Game.UserResources;
using WarehouseKeeper.Levels;
using WarehouseKeeper.Repositories;
using WarehouseKeeper.Repositories.FileSave;
using WarehouseKeeper.Repositories.FileSave.Implementation;
using WarehouseKeeper.Repositories.Implementation;
using Zenject;

namespace WarehouseKeeper.DependencyInjection.ProjectInstallers
{
internal class SaveSystemInstaller : Installer<SaveSystemInstaller>
{
    private static readonly string LevelsSettingsResourcesDirectory = "Levels/";
    private static readonly string UserDataDirectory = Application.persistentDataPath + "/UserData/";
    
#if UNITY_EDITOR
    private static readonly string LevelsDirectory = Application.persistentDataPath + "/UserData/Levels/";
#elif UNITY_IOS
    private static readonly string LevelsDirectory = Application.persistentDataPath + "/UserData/Levels/";
#elif UNITY_ANDROID
    private static readonly string LevelsDirectory = Application.persistentDataPath + "/UserData/Levels/";
#elif UNITY_STANDALONE_OSX
    private static readonly string LevelsDirectory = Application.persistentDataPath + "/UserData/Levels/";
#else
    private static readonly string LevelsDirectory = Application.persistentDataPath + "/UserData/Levels/";
#endif
    
    public override void InstallBindings()
    {
        Container.Bind<ISaveFile>().To<BinarySave>().AsSingle();
        
        Container.Bind<IRepository<LevelData>>().To<FileRepositoryManager<LevelData>>().AsSingle().NonLazy();
        Container.Bind<string>().FromInstance(LevelsDirectory).WhenInjectedInto<IRepository<LevelData>>();
        
        Container.Bind<IRepository<LevelSettings>>().To<StaticResourcesRepositoryManager<LevelSettings>>().AsSingle().NonLazy();
        Container.Bind<string>().FromInstance(LevelsSettingsResourcesDirectory).WhenInjectedInto<StaticResourcesRepositoryManager<LevelSettings>>();
        
        Container.Bind<IRepository<UserData>>().To<FileRepositoryManager<UserData>>().AsSingle().NonLazy();
        Container.Bind<string>().FromInstance(UserDataDirectory).WhenInjectedInto<FileRepositoryManager<UserData>>();
    }
}
}