using UnityEngine;
using WarehouseKeeper.Extension;
using WarehouseKeeper.Gui.Windows;
using WarehouseKeeper.Gui.Windows.Transitions;
using WarehouseKeeper.Gui.Windows.WindowsFactories;
using Zenject;

namespace WarehouseKeeper.DependencyInjection.ProjectInstallers
{
public class ProjectGuiInstaller: Installer<ProjectGuiInstaller>
{
    private const string ResourcesWindowSettingsPath = "WindowSettings";
    
    public override void InstallBindings()
    {
        Container.Bind<IWindowTransition>().To<VerticalTransition>().AsSingle();
        Container.Bind<WindowSettings>().FromMethod(LoadInputSettingsFromResources).AsSingle().NonLazy();
    }
    
    private WindowSettings LoadInputSettingsFromResources()
    {
        var so = Resources.Load<WindowSettingsSo>(ResourcesWindowSettingsPath);
        if (so == null)
        {
            Log.WriteError($"Can't load input so settings. Path to so: {ResourcesWindowSettingsPath}");

            return default;
        }

        return so.windowSettings;
    }
}
}