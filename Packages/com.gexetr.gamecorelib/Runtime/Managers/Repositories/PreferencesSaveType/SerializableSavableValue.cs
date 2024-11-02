using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Game.PreferencesSaveType
{
internal class SerializableSavableValue<T> : BaseSavableValue<T>
{
    public SerializableSavableValue(string playerPrefsPath, T defaultValue = default) 
        : base(playerPrefsPath, defaultValue)
    {
    }

    //ToDo: refactor(using(...)) ant testing this elements
    protected override T LoadValue(ref string path)
    {
        var stringToDeserialize = PlayerPrefs.GetString(path, "");

        var bytes = Convert.FromBase64String(stringToDeserialize);
        var memoryStream = new MemoryStream(bytes);
        var bf = new BinaryFormatter();

        return (T)bf.Deserialize(memoryStream);
    }

    //ToDo: refactor(using(...)) ant testing this elements
    protected override void SaveValue(ref string path)
    {
        var memoryStream = new MemoryStream();
        var bf = new BinaryFormatter();
        bf.Serialize(memoryStream, cachedValue);
        var stringToSave = Convert.ToBase64String(memoryStream.ToArray());

        PlayerPrefs.SetString(path, stringToSave);
    }
}
}