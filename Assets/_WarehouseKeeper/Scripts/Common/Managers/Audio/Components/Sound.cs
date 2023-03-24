using System;
using UnityEngine;
using UnityEngine.Audio;

namespace WarehouseKeeper.Audio
{
[Serializable]
internal class Sound
{
    [field: SerializeField] public AudioClip Clip { get; set; }
    [field: SerializeField] public AudioMixerGroup CustomOutput { get; set; }
    [field: SerializeField] public bool Loop { get; set; } = false;
    [SerializeField] private bool bypassEffects = false;
    [SerializeField] private bool bypassListenerEffects = false;
    [SerializeField] private bool bypassReverbZones = false;
    [SerializeField, Range(0, 255)] private int priority = 128;
    [SerializeField, Range(0, 1f)] private float volume = 1f;
    [SerializeField, Range(-3f, 3f)] private float pitch = 1f;
    [SerializeField, Range(-1f, 1f)] private float stereoPan = 0f;
    [SerializeField, Range(0, 1f)] private float spatialBlend = 0f;
    [SerializeField, Range(0, 1.1f)] private float reverbZoneMix = 1f;
    [SerializeField, Range(0, 5f)] private float dopplerLevel = 1f;
    [SerializeField, Range(0, 5f)] private int spread = 0;
    [SerializeField] private AudioRolloffMode volumeRolloff = AudioRolloffMode.Logarithmic;
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private float maxDistance = 500f;
    [field: SerializeField] public FadeSettings Fade { get; private set; }

    internal void SetupSource(Source source)
    {
        var audio = source.AudioSource;
        audio.clip = Clip;
        audio.outputAudioMixerGroup = CustomOutput;
        audio.bypassEffects = bypassEffects;
        audio.bypassListenerEffects = bypassListenerEffects;
        audio.bypassReverbZones = bypassReverbZones;
        audio.loop = Loop;
        audio.priority = priority;
        audio.volume = volume;
        audio.pitch = pitch;
        audio.panStereo = stereoPan;
        audio.spatialBlend = spatialBlend;
        audio.reverbZoneMix = reverbZoneMix;
        audio.dopplerLevel = dopplerLevel;
        audio.spread = spread;
        audio.rolloffMode = volumeRolloff;
        audio.minDistance = minDistance;
        audio.maxDistance = maxDistance; 
    }

    public void CloneSettingsTo(Sound sound)
    {
        sound.bypassEffects = bypassEffects;
        sound.bypassListenerEffects = bypassListenerEffects;
        sound.bypassReverbZones = bypassReverbZones;
        sound.priority = priority;
        sound.volume = volume;
        sound.pitch = pitch;
        sound.stereoPan = stereoPan;
        sound.spatialBlend = spatialBlend;
        sound.reverbZoneMix = reverbZoneMix;
        sound.dopplerLevel = dopplerLevel;
        sound.spread = spread;
        sound.volumeRolloff = volumeRolloff;
        sound.minDistance = minDistance;
        sound.maxDistance = maxDistance;
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
    internal struct FadeSettings
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