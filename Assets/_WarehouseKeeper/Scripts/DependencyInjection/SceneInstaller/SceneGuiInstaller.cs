using UnityEngine;
using WarehouseKeeper.Gui.Windows;
using WarehouseKeeper.Gui.Windows.WindowsFactories;
using Zenject;

namespace WarehouseKeeper.DependencyInjection.SceneInstallers
{
public class SceneGuiInstaller : Installer<Transform, SceneGuiInstaller>
{
    private Transform _uiRoot;

    public SceneGuiInstaller(Transform uiRoot)
    {
        _uiRoot = uiRoot;
    }

    public override void InstallBindings()
    {
        Container.BindInterfacesTo<WindowsManager>().AsSingle();
        Container.Bind<IWindowFactory>().To<WindowsFactory>().AsSingle();
        Container.Bind<Transform>().FromInstance(_uiRoot).WhenInjectedInto<WindowsManager>();
    }
}
}