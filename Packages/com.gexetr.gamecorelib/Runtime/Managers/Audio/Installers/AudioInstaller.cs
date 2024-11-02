using Game.Audio.Managers;
using Game.Factories;

namespace Game.Audio.Installers
{
public class AudioInstaller
{
    private static readonly AudioSettings _settings;
    private const string ResourcesSettingsPath = "Audio/AudioSettings";

    static AudioInstaller()
    {
        _settings = LoadSettingsFromResources();
    }

    public static IAudioManager UnityAudio(IFactoryGameObjects factory) => new UnityAudioManager(factory, _settings);
    
    private static AudioSettings LoadSettingsFromResources()
    {
        var so = UnityEngine.Resources.Load<AudioSettings>(ResourcesSettingsPath);
        if (so == null)
        {
            Log.Error($"Can't load localization so settings. Path to so: {ResourcesSettingsPath}");

            return default;
        }

        return so;
    }
}
}