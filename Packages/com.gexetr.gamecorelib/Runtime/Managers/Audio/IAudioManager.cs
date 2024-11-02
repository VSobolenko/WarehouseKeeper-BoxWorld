using UnityEngine;

namespace Game.Audio
{
public interface IAudioManager
{
    bool SoundEnabled { get; set; }
    bool MusicEnabled { get; set; }
    
    AudioSound Play(Sound settings, AudioClip clip, Source source, bool loop, ChanelType type);
    AudioSound Play(Sound sound, Source source, bool loop, ChanelType type);
    bool Stop(AudioSound audioSound);
    void SetAudioListenerWorldPosition(Vector3 position);
}
}