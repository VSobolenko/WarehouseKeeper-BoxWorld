using System;
using UnityEngine;

namespace Game.PreferencesSaveType
{
internal abstract class BaseSavableValue<T> : IDisposable
{
    private string _playerPrefsPath;
    private bool _valueInitialize;
    protected readonly T defaultValue;
    protected T cachedValue;
    private static event Action OnValueChangedOutside;
    
    protected BaseSavableValue(string playerPrefsPath, T defaultValue = default)
    {
        if (string.IsNullOrEmpty(playerPrefsPath))
            throw new Exception("Empty playerPrefsPath in savableValue");
        
        _playerPrefsPath = playerPrefsPath;
        this.defaultValue = defaultValue;
        _valueInitialize = false;
        OnValueChangedOutside += ProcessValueChangedOutside;
    }

    public T Value
    {
        get
        {
            if (_valueInitialize) 
                return cachedValue;

            var containsSaves = ContainsSaves();
            cachedValue = containsSaves ? LoadValue(ref _playerPrefsPath) : defaultValue;
            _valueInitialize = true;

            if (containsSaves == false)
                SaveValue(ref _playerPrefsPath);

            return cachedValue;
        }
        set
        {
            cachedValue = value;
            SaveValue(ref _playerPrefsPath);
            _valueInitialize = true;
        }
    }

    protected abstract T LoadValue(ref string path);
    
    protected abstract void SaveValue(ref string path);

    private bool ContainsSaves() => PlayerPrefs.HasKey(_playerPrefsPath);

    public void Reset()
    {
        PlayerPrefs.DeleteKey(_playerPrefsPath);
        _valueInitialize = false;
    }

    private void ProcessValueChangedOutside()
    {
        _valueInitialize = false;
    }
    
    public void Dispose()
    {
        OnValueChangedOutside -= ProcessValueChangedOutside;
    }
    
    public static void ResetAll()
    {
        PlayerPrefs.DeleteAll();
        OnValueChangedOutside?.Invoke();
    }
    
    public static void SaveAll()
    {
        PlayerPrefs.Save();
        OnValueChangedOutside?.Invoke();
    }
}
}