using UnityEngine;
using WarehouseKeeper.Extension;
using WarehouseKeeper.Inputs;
using WarehouseKeeper.Inputs.Implementation;
using WarehouseKeeper.Localizations;
using WarehouseKeeper.Localizations.Components;
using WarehouseKeeper.Localizations.Managers;
using Zenject;

namespace WarehouseKeeper.DependencyInjection.ProjectInstallers
{
public class LocalizationInstaller : Installer<LocalizationInstaller>
{
    private const string ResourcesSettingsPath = "Localization/LocalizationSettings";
    
    public override void InstallBindings()
    {
        Container.Bind<ILocalizationManager>().To<LocalizationManager>().AsSingle().NonLazy();
        Container.Bind<LocalizationSettings>().FromMethod(LoadSettingsFromResources).AsSingle().NonLazy();
    }

    private LocalizationSettings LoadSettingsFromResources()
    {
        var so = Resources.Load<LocalizationSettings>(ResourcesSettingsPath);
        if (so == null)
        {
            Log.WriteError($"Can't load localization so settings. Path to so: {ResourcesSettingsPath}");

            return default;
        }

        return so;
    }
}
}