using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Game.Audio
{
[Serializable]
public class Sound
{
    [field: SerializeField] public AudioClip Clip { get; set; }
    [field: SerializeField] public AudioMixerGroup CustomOutput { get; set; }
    [field: SerializeField] public bool Loop { get; set; } = false;
    [SerializeField] private bool _bypassEffects = false;
    [SerializeField] private bool _bypassListenerEffects = false;
    [SerializeField] private bool _bypassReverbZones = false;
    [SerializeField, Range(0, 255)] private int _priority = 128;
    [SerializeField, Range(0, 1f)] private float _volume = 1f;
    [SerializeField, Range(-3f, 3f)] private float _pitch = 1f;
    [SerializeField, Range(-1f, 1f)] private float _stereoPan = 0f;
    [SerializeField, Range(0, 1f)] private float _spatialBlend = 0f;
    [SerializeField, Range(0, 1.1f)] private float _reverbZoneMix = 1f;
    [SerializeField, Range(0, 5f)] private float _dopplerLevel = 1f;
    [SerializeField, Range(0, 5f)] private int _spread = 0;
    [SerializeField] private AudioRolloffMode _volumeRolloff = AudioRolloffMode.Logarithmic;
    [SerializeField] private float _minDistance = 1f;
    [SerializeField] private float _maxDistance = 500f;
    [field: SerializeField] public FadeSettings Fade { get; private set; }

    internal void SetupSource(Source source)
    {
        var audio = source.AudioSource;
        audio.clip = Clip;
        audio.outputAudioMixerGroup = CustomOutput;
        audio.bypassEffects = _bypassEffects;
        audio.bypassListenerEffects = _bypassListenerEffects;
        audio.bypassReverbZones = _bypassReverbZones;
        audio.loop = Loop;
        audio.priority = _priority;
        audio.volume = _volume;
        audio.pitch = _pitch;
        audio.panStereo = _stereoPan;
        audio.spatialBlend = _spatialBlend;
        audio.reverbZoneMix = _reverbZoneMix;
        audio.dopplerLevel = _dopplerLevel;
        audio.spread = _spread;
        audio.rolloffMode = _volumeRolloff;
        audio.minDistance = _minDistance;
        audio.maxDistance = _maxDistance; 
    }

    public void CloneSettingsTo(Sound sound)
    {
        sound._bypassEffects = _bypassEffects;
        sound._bypassListenerEffects = _bypassListenerEffects;
        sound._bypassReverbZones = _bypassReverbZones;
        sound._priority = _priority;
        sound._volume = _volume;
        sound._pitch = _pitch;
        sound._stereoPan = _stereoPan;
        sound._spatialBlend = _spatialBlend;
        sound._reverbZoneMix = _reverbZoneMix;
        sound._dopplerLevel = _dopplerLevel;
        sound._spread = _spread;
        sound._volumeRolloff = _volumeRolloff;
        sound._minDistance = _minDistance;
        sound._maxDistance = _maxDistance;
        sound.Fade = Fade;
    }
    
    private void SetupDefaultFadeSettings()
    {
        Fade = new FadeSettings
        {
            //ToDo: Add global parameters
            //useGlobalParameters = true,
            enableUpFade = true,
            upFadeDuration = 0.5f,
            enableDownFade = true,
            downFadeDuration = 0.5f,
        };
    }
    
    [Serializable]
    public struct FadeSettings
    {
        //ToDo: Add global parameters
        //public bool useGlobalParameters;
        
        public bool enableUpFade;
        public float upFadeDuration;
        
        public bool enableDownFade;
        public float downFadeDuration;
    }
}
}