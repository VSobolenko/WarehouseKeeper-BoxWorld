using Game.Installers.Ads;
using Game.Installers.AssetContent;
using Game.Installers.Audio;
using Game.Installers.Factories;
using Game.Installers.GUI;
using Game.Installers.Inputs;
using Game.Installers.Localizations;
using Game.Installers.Repositories;
using Game.Installers.Shops;
using WarehouseKeeper.Directors.Game.Analytics.Signals;
using WarehouseKeeper.Directors.Game.UserResources;
using WarehouseKeeper.Levels;
using Zenject;

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
        SignalBusInstaller.Install(Container);
        SaveSystemInstaller<LevelData, LevelSettings, UserData>.Install(Container);
        InputInstaller.Install(Container);
        LocalizationInstaller.Install(Container);
        ProjectGuiInstaller.Install(Container);
        AudioInstaller.Install(Container);
        ShopInstaller.Install(Container);
        AdsInstaller.Install(Container);
        FactoryInstaller.Install(Container);
        AddressablesInstaller.Install(Container);
        
        Container.BindInterfacesAndSelfTo<LevelRepositoryDirector>().AsSingle();

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
        Container.DeclareSignal<ResetProgress>();
        Container.DeclareSignal<UnlockLevelByAmber>();
    }
}
}