using UnityEngine;

namespace Game.PreferencesSaveType
{
internal class StringSavableValue : BaseSavableValue<string>
{
    public StringSavableValue(string playerPrefsPath, string defaultValue = default) 
        : base(playerPrefsPath, defaultValue)
    {
    }

    protected override string LoadValue(ref string path) => PlayerPrefs.GetString(path, defaultValue);

    protected override void SaveValue(ref string path) => PlayerPrefs.SetString(path, cachedValue);
}
}