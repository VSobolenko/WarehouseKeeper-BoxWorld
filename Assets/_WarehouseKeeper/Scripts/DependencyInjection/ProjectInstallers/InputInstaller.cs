using UnityEngine;
using WarehouseKeeper.Extension;
using WarehouseKeeper.Inputs;
using WarehouseKeeper.Inputs.Implementation;
using Zenject;

namespace WarehouseKeeper.DependencyInjection.ProjectInstallers
{
public class InputInstaller : Installer<InputInstaller>
{
    private const string ResourcesSettingsPath = "InputSettings";
    
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<InputManager>().AsSingle();
        Container.Bind<SwipeManager>().AsSingle();
        Container.Bind<InputSettings>().FromMethod(LoadSettingsFromResources).AsSingle().NonLazy();
    }

    private InputSettings LoadSettingsFromResources()
    {
        var so = Resources.Load<InputSettingsSo>(ResourcesSettingsPath);
        if (so == null)
        {
            Log.WriteError($"Can't load input so settings. Path to so: {ResourcesSettingsPath}");

            return default;
        }

        return so.inputSettings;
    }
}
}