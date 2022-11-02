using WarehouseKeeper.Levels;
using Zenject;

namespace WarehouseKeeper.DependencyInjection.Installers
{
public class CommonInstaller : Installer<CommonInstaller>
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<LevelDirector>().AsSingle();
    }
}
}