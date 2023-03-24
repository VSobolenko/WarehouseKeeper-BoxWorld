using System;
using WarehouseKeeper.AssetContent;
using WarehouseKeeper.AssetContent.Implementation;
using WarehouseKeeper.DependencyInjection.ProjectInstallers;
using WarehouseKeeper.Directors.Game.Analytics.Signals;
using WarehouseKeeper.Factories;
using WarehouseKeeper.Factories.Implementation;
using WarehouseKeeper.Shops;
using Zenject;

namespace WarehouseKeeper.DependencyInjection
{
public class ProjectInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        InstallBaseManagers();
        InstallManagers();
        DeclareSignals();
    }

    private void InstallBaseManagers()
    {
        SignalBusInstaller.Install(Container);
        SaveSystemInstaller.Install(Container);
        CommonInstaller.Install(Container);
        InputInstaller.Install(Container);
        LocalizationInstaller.Install(Container);
        ProjectGuiInstaller.Install(Container);
        AudioInstaller.Install(Container);
        ShopInstaller.Install(Container);
        AdsInstaller.Install(Container);
        
        Container.Bind<IFactoryGameObjects>().To<GameObjectsFactory>().AsSingle();
        Container.Bind<IAddressablesManager>().To<AddressablesManager>().AsSingle();
    }

    private void InstallManagers()
    {
        
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