using UnityEngine;

namespace Game.Audio
{
public class AudioSound
{
    public Sound Audio { get; }
    public Source Source { get; }

    private readonly IAudioManager _audioManager;

    public AudioSound(Sound audio, Source source, IAudioManager audioManager)
    {
        Audio = audio;
        Source = source;
        _audioManager = audioManager;

        Initialize();
    }

    private void Initialize()
    {
        Audio.SetupSource(Source);
        Source.AudioSource.Play();
        if (Audio.Fade.enableUpFade)
            Source.FadeSource(Audio.Fade.upFadeDuration, 1f);
        if (Audio.Loop == false)
            Source.EnableAutoAction(Audio.Clip.length, Stop);
    }
    
    public void SetWorldPosition(Vector3 position)
    {
        Source.gameObject.transform.position = position;
    }

    public void StopFaded()
    {
        if (Audio.Fade.enableDownFade)
            Source.FadeSource(Audio.Fade.downFadeDuration, 0f, Stop);
        else
            Stop();
    }

    public void Stop()
    {
        Source.StopSource();
        _audioManager.Stop(this);
        if (Source.Pool != null)
            Source.Release();
    }
}
}