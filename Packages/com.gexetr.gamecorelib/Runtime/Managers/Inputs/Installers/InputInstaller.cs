using Game.Inputs.Managers;
using UnityEngine;

namespace Game.Inputs.Installers
{
public class InputInstaller
{
    private const string ResourcesSettingsPath = "InputSettings";

    private static InputSettings _settings;

    static InputInstaller()
    {
        _settings = LoadSettingsFromResources();
    }
    
    public static IInputManager Manager() => new InputManager();
    public static SwipeDetector Swipe(IInputManager manager) => new SwipeDetector(manager, _settings);
    
    private static InputSettings LoadSettingsFromResources()
    {
        var so = Resources.Load<InputSettingsSo>(ResourcesSettingsPath);

        if (so != null) 
            return so.inputSettings;
        
        Log.Error($"Can't load input so settings. Path to so: {ResourcesSettingsPath}");

        return default;

    }
}
}