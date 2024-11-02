using UnityEngine;

namespace Game.PreferencesSaveType
{
internal class BoolSavableValue : BaseSavableValue<bool>
{
    public BoolSavableValue(string playerPrefsPath, bool defaultValue = default) 
        : base(playerPrefsPath, defaultValue)
    {
    }

    protected override bool LoadValue(ref string path)
    {
        var intType = PlayerPrefs.GetInt(path, Bool2Int(defaultValue));

        return Int2Bool(intType);
    }

    protected override void SaveValue(ref string path) => PlayerPrefs.SetInt(path, Bool2Int(cachedValue));

    private static bool Int2Bool(int value) => value > 0;
    
    private static int Bool2Int(bool value) => value ? 1 : 0;
}
}