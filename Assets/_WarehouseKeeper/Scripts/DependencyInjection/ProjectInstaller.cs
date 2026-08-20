using System;
using System.Reflection;
using System.Reflection.Emit;
using Game.Ads;
using Game.Ads.Installers;
using Game.AssetContent;
using Game.AssetContent.Installers;
using Game.Audio;
using Game.Audio.Installers;
using Game.Factories;
using Game.GUI.Windows;
using Game.GUI.Windows.Factories;
using Game.Inputs;
using Game.Inputs.Installers;
using Game.IO.Installers;
using Game.Localizations;
using Game.Localizations.Installers;
using Game.Repositories;
using Game.Repositories.Installers;
using Game.Shops;
using UnityEngine;
using WarehouseKeeper.DependencyInjection.ZenjectDependency;
using WarehouseKeeper.Directors.Game.Analytics.Signals;
using WarehouseKeeper.Directors.Game.UserResources;
using WarehouseKeeper.Levels;
using WarehouseKeeper._WarehouseKeeper.Scripts.Shops.Monetization.Purchasing.IAP;
using WarehouseKeeper._WarehouseKeeper.Scripts.Shops.Monetization.Purchasing.IAP.UnityServices;
using WarehouseKeeper._WarehouseKeeper.Scripts.Shops.Monetization.Purchasing;
using WarehouseKeeper.UI.Windows.MainWindows;
using Zenject;
using PurchasingUnityServicesManager = WarehouseKeeper._WarehouseKeeper.Scripts.Shops.Monetization.Purchasing.IAP.UnityServices.UnityServicesManager;

namespace WarehouseKeeper.DependencyInjection
{
public class ProjectInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        InstallBaseManagers();
        DeclareSignals();
    }

    private void InstallBaseManagers()
    {
        const string levelsSettingsResourcesDirectory = "Levels/";
        var levelDirectory = Application.persistentDataPath + "/UserData/Levels/";
        var userDataDirectory = Application.persistentDataPath + "/UserData/";
        ;

        var fileSaver = SaveSystemInstaller.FileSaver();
        var levelDataRepository = RepositoryInstaller.File<LevelData>(levelDirectory, fileSaver);
        var userDataRepository = RepositoryInstaller.File<UserData>(userDataDirectory, fileSaver);
        var levelSettingsRepository =
            RepositoryInstaller.StaticResources<LevelSettings>(levelsSettingsResourcesDirectory, fileSaver);
        Container.Bind<IRepository<LevelData>>().FromInstance(levelDataRepository).AsSingle().NonLazy();
        Container.Bind<IRepository<UserData>>().FromInstance(userDataRepository).AsSingle().NonLazy();
        Container.Bind<IRepository<LevelSettings>>().FromInstance(levelSettingsRepository).AsSingle().NonLazy();
        Container.Bind<IResourceManager>().FromInstance(ResourceManagerInstaller.Addressable()).AsSingle();
        Container.Bind<IFactoryGameObjects>().To<DependencyInjectionFactory>().AsSingle();
        var inputManager = InputInstaller.Manager();
        var swipe = InputInstaller.Swipe(inputManager);
        Container.Bind<IInputManager>().FromInstance(inputManager).AsSingle();
        Container.Bind<SwipeDetector>().FromInstance(swipe).AsSingle();

        var adDetails = new AdDetails
        {
            sdkKey = "6nhv2UISHVtgZNl9Ml2fwH5v-MxHFBoVkybv1no4mCaTKIMsxmCNBLJiNBnGLyBeTSV9dCWt2u-I3w1r9wQ_kN",
            rewardedAdUnitId = "79c242342de99d2d",
            interstitialAdUnitId = "9c4f76c0b6c71a2e",
        };
        var adManager = AdsInstaller.Plug();
        Container.Bind<IAdsManager>().FromInstance(adManager).AsSingle().NonLazy();

        var localization = LocalizationInstaller.Manager();
        Container.Bind<ILocalizationManager>().FromInstance(localization).AsSingle().NonLazy();

        var factory = Container.Resolve<IFactoryGameObjects>();
        var audioManager = AudioInstaller.UnityAudio(factory);
        Container.Bind<IAudioManager>().FromInstance(audioManager).AsSingle().NonLazy();
        var iapCollection = Resources.Load<IAPConfigurationCollection>($"Purchasing/{nameof(IAPConfigurationCollection)}");
        if (iapCollection == null)
        {
            Debug.LogWarning($"Missing Resources/Purchasing/{nameof(IAPConfigurationCollection)}. Using empty IAP catalog fallback.");
            iapCollection = ScriptableObject.CreateInstance<IAPConfigurationCollection>();
            iapCollection.products = Array.Empty<IAPConfigurationData>();
        }

        Container.Bind<IAPConfigurationCollection>().FromInstance(iapCollection).AsSingle().NonLazy();
        Container.Bind<IUnityServicesInitializer>().To<PurchasingUnityServicesManager>().AsSingle().NonLazy();
        Container.Bind<IShopCatalog>().To<ResourcesShopCatalog>().AsSingle().NonLazy();
        Container.Bind<IPurchasingDirector>().To<PurchasingDirectorV5>().AsSingle().NonLazy();

        // SaveSystemInstallerZenject<LevelData, LevelSettings, UserData>.Install(Container);
        // AddressablesInstaller.Install(Container);
        // FactoryInstallerZenject.Install(Container);
        // InputInstallerZenject.Install(Container);
        // ProjectGuiInstaller.Install(Container);
        // AdsInstallerZenject.Install(Container);
        // LocalizationInstallerZenject.Install(Container);
        // AudioInstallerZenject.Install(Container);
        // ShopInstallerZenject.Install(Container);

        SignalBusInstaller.Install(Container);
        Container.BindInterfacesAndSelfTo<LevelRepositoryDirector>().AsSingle();

        var tickWrapper = new TickWrapper();
        tickWrapper.items.Add(inputManager);
        Container.Bind<ITickable>().FromInstance(tickWrapper).NonLazy();
    }

    private void DeclareSignals()
    {
        Container.DeclareSignal<LevelGoHome>();
        Container.DeclareSignal<LevelRestart>();
        Container.DeclareSignal<LevelStart>();
        Container.DeclareSignal<LevelVictory>();
        Container.DeclareSignal<ActivateHint>();
        Container.DeclareSignal<PurchaseAmber>();
        Container.DeclareSignal<PurchaseProduct>();
        Container.DeclareSignal<ShopEvent>();
        Container.DeclareSignal<ResetProgress>();
        Container.DeclareSignal<UnlockLevelByAmber>();
    }
}
}