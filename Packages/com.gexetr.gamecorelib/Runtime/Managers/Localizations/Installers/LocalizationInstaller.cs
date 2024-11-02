using Game.Localizations.Components;
using Game.Localizations.Managers;
using UnityEngine;

namespace Game.Localizations.Installers
{
public class LocalizationInstaller
{
    private const string ResourcesSettingsPath = "Localization/LocalizationSettings";

    internal static readonly LocalizationSettings Settings;

    static LocalizationInstaller()
    {
        Settings = LoadSettingsFromResources();
    }

    public static ILocalizationManager Manager() => new LocalizationManager(Settings);
    
    private static LocalizationSettings LoadSettingsFromResources()
    {
        var so = Resources.Load<LocalizationSettings>(ResourcesSettingsPath);

        if (so != null) 
            return so;
        Log.Error($"Can't load localization so settings. Path to so: {ResourcesSettingsPath}");

        return default;

    }
}
}