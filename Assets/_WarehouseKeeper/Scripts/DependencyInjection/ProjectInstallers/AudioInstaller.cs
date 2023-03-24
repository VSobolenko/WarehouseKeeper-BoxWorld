using UnityEngine;
using WarehouseKeeper.Audio;
using WarehouseKeeper.Extension;
using Zenject;
using AudioSettings = WarehouseKeeper.Audio.AudioSettings;

namespace WarehouseKeeper.DependencyInjection.ProjectInstallers
{
public class AudioInstaller : Installer<AudioInstaller>
{
    private const string ResourcesSettingsPath = "Audio/AudioSettings";
    
    public override void InstallBindings()
    {
        Container.Bind<IAudioManager>().To<UnityAudioManager>().AsSingle().NonLazy();
        Container.Bind<AudioSettings>().FromMethod(LoadSettingsFromResources).AsSingle().NonLazy();
    }

    private AudioSettings LoadSettingsFromResources()
    {
        var so = Resources.Load<AudioSettings>(ResourcesSettingsPath);
        if (so == null)
        {
            Log.WriteError($"Can't load localization so settings. Path to so: {ResourcesSettingsPath}");

            return default;
        }

        return so;
    }
}
}