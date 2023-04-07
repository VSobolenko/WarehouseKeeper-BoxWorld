using Game.Audio;
using Zenject;

namespace WarehouseKeeper.Directors.Game.Audio
{
internal class AudioDirector : IInitializable
{
    private readonly IAudioManager _audioManager;
    private readonly AudioFactory _audioFactory;
    private readonly AudioSo _audioData;

    private AudioSound _lobbySound;
    private AudioSound _gameSound;
    
    public AudioDirector(AudioFactory audioFactory, IAudioManager audioManager, ResourcesDirector resourcesDirector)
    {
        _audioFactory = audioFactory;
        _audioManager = audioManager;
        _audioData = resourcesDirector.AudioData;
    }

    //AudioMixer.SetFloat not working in Awake()
    public void Initialize()
    {
        var music = _audioManager.MusicEnabled;
        var sound = _audioManager.SoundEnabled;
        
        _audioManager.MusicEnabled = music;
        _audioManager.SoundEnabled = sound;
    }
    
    public void PlayLobbyBackground()
    {
        var source = _lobbySound == null ? _audioFactory.GetSource() : _lobbySound.Source;
        _lobbySound = _audioManager.Play(_audioData.LobbySettings, _audioData.LobbyBackground, source, true, ChanelType.Music);
    }
    
    public void StopLobbyBackground()
    {
        if (_lobbySound == null)
            return;
        _lobbySound.StopFaded();
        _lobbySound = null;
    }
    
    public void PlayGameBackground()
    {
        var source = _gameSound == null ? _audioFactory.GetSource() : _gameSound.Source;
        _gameSound = _audioManager.Play(_audioData.GameSettings, _audioData.GameBackground, source, true, ChanelType.Music);
    }
    
    public void StopGameBackground()
    {
        if (_gameSound == null)
            return;
        _gameSound.StopFaded();
        _gameSound = null;
    }

    public void PlayButtonClick()
    {
        var source = _audioFactory.GetSource();
        _audioManager.Play(_audioData.UISettings, _audioData.ButtonClick, source, false, ChanelType.Sound);
    }
}
}