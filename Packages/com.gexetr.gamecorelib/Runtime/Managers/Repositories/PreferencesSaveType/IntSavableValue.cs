using UnityEngine;

namespace Game.PreferencesSaveType
{
internal class IntSavableValue : BaseSavableValue<int>
{
    public IntSavableValue(string playerPrefsPath, int defaultValue = default) 
        : base(playerPrefsPath, defaultValue)
    {
    }

    protected override int LoadValue(ref string path) => PlayerPrefs.GetInt(path, defaultValue);

    protected override void SaveValue(ref string path) => PlayerPrefs.SetInt(path, cachedValue);
}
}