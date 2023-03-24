using System;
using UnityEngine;
using WarehouseKeeper.Extension;

namespace WarehouseKeeper.Repositories.PreferencesSaveType
{
internal class EnumSavableValue<T> : BaseSavableValue<T> where T: Enum
{
    public EnumSavableValue(string playerPrefsPath, T defaultValue = default) : 
        base(playerPrefsPath, defaultValue)
    {
    }

    protected override T LoadValue(ref string path)
    {
        try
        {
            var stringType = PlayerPrefs.GetString(path, defaultValue.ToString());

            return Enum.Parse<T>(stringType);
        }
        catch (Exception e)
        {
            Log.WriteError($"Load enum fail: {e.Message}");

            return defaultValue;
        }
    }

    protected override void SaveValue(ref string path) => PlayerPrefs.SetString(path, cachedValue.ToString());
}
}