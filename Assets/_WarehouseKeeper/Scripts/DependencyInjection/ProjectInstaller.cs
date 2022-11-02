using WarehouseKeeper.AssetContent;
using WarehouseKeeper.AssetContent.Managers;
using WarehouseKeeper.DependencyInjection.Installers;
using WarehouseKeeper.Factories;
using WarehouseKeeper.Factories.Managers;
using Zenject;

namespace WarehouseKeeper.DependencyInjection
{
public class ProjectInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);
        SaveSystemInstaller.Install(Container);
        CommonInstaller.Install(Container);

        Container.Bind<IFactoryGameObjects>().To<GameObjectsFactory>().AsSingle();
        Container.Bind<IAddressablesManager>().To<AddressablesManager>().AsSingle();
    }
}
}