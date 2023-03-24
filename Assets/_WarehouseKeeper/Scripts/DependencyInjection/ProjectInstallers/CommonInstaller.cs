using WarehouseKeeper.Levels;
using Zenject;

namespace WarehouseKeeper.DependencyInjection.ProjectInstallers
{
public class CommonInstaller : Installer<CommonInstaller>
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<LevelRepositoryDirector>().AsSingle();
    }
}
}