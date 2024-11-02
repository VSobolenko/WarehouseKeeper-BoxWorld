using System.Collections.Generic;
using System.Linq;
using Game.Factories;
using Game.InternalData;
using Game.PreferencesSaveType;
using UnityEngine;

namespace Game.Audio.Managers
{
/// <summary>
/// Bad solution
/// </summary>
internal class UnityAudioManager : IAudioManager
{
    private readonly AudioSettings _audioSettings;
    private readonly AudioListener _listener;
    private readonly Transform _audioRoot;
    private readonly HashSet<AudioSound> _sounds = new(5);
    private readonly BoolSavableValue _saveMusic = new("Music_status", true);
    private readonly BoolSavableValue _saveSound = new("Sound_status", true);
    
    public UnityAudioManager(IFactoryGameObjects factoryGameObjects, AudioSettings audioSettings)
    {
        _audioSettings = audioSettings;

        var root = factoryGameObjects.InstantiateEmpty();
        _listener = factoryGameObjects.InstantiateAndAddNewComponent<AudioListener>(root.transform);
        _audioRoot = factoryGameObjects.InstantiateEmpty(root.transform).transform;

        if (Application.isEditor)
        {
            root.name = $"{GameData.Identifier}.Audio";
            _listener.name = "[Listener]";
            _audioRoot.name = "[Audio]";
        }
    }

    public bool SoundEnabled
    {
        get => _saveMusic.Value;
        set
        {
            _saveMusic.Value = value;
            SetActiveMixerVolume(ChanelType.Sound, value);
        }
    }
    
    public bool MusicEnabled
    {
        get => _saveSound.Value;
        set
        {
            _saveSound.Value = value;
            SetActiveMixerVolume(ChanelType.Music, value);
        }
    }

    public AudioSound Play(Sound settings, AudioClip clip, Source source, bool loop, ChanelType type)
    {
        var sound = new Sound
        {
            Clip = clip,
            Loop = loop,
            CustomOutput = _audioSettings.Mixers.First(x => x.mixerType == type).mixerGroup,
        };
        settings.CloneSettingsTo(sound);
        source.transform.SetParent(_audioRoot);
        var audioSound = new AudioSound(sound, source, this);
        _sounds.Add(audioSound);

        return audioSound;
    }

    public AudioSound Play(Sound sound, Source source, bool loop, ChanelType type)
    {
        return Play(sound, sound.Clip, source, loop, type);
    }

    public bool Stop(AudioSound audioSound)
    {
        var result = _sounds.Remove(audioSound);
        if (result == false)
            Log.InternalError();

        return result;
    }


    public void SetAudioListenerWorldPosition(Vector3 position)
    {
        _listener.transform.position = position;
    }

    private void SetActiveMixerVolume(ChanelType type, bool status)
    {
        var mixerData = _audioSettings.Mixers.First(x => x.mixerType == type);
        mixerData.mixerGroup.audioMixer.SetFloat($"{type}Volume", status ? 0f : -80f);
        mixerData.mixerGroup.audioMixer.GetFloat($"{type}Volume", out var value);
    }
}
}