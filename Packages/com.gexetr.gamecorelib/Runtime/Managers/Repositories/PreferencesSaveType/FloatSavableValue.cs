using UnityEngine;

namespace Game.PreferencesSaveType
{
internal class FloatSavableValue : BaseSavableValue<float>
{
    public FloatSavableValue(string playerPrefsPath, float defaultValue = default) 
        : base(playerPrefsPath, defaultValue)
    {
    }

    protected override float LoadValue(ref string path) => PlayerPrefs.GetFloat(path, defaultValue);

    protected override void SaveValue(ref string path) => PlayerPrefs.SetFloat(path, cachedValue);
}
}