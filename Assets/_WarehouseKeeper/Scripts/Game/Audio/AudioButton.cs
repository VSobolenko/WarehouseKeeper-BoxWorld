using System;
using UnityEngine;
using UnityEngine.UI;
using WarehouseKeeper.Extension;
using Zenject;

namespace WarehouseKeeper.Directors.Game.Audio
{
[DisallowMultipleComponent]
public class AudioButton : MonoBehaviour
{
    [SerializeField] private Button _targetButton;

    [Inject] private AudioDirector _audioDirector;

    //ToDo: Add custom sound
    //[SerializeField] private WarehouseKeeper.Audio.Sound _customSound;

    private void Start()
    {
        if (_targetButton == null || _audioDirector == null)
        {
            Log.InternalError();
            return;
        }
        
        _targetButton.onClick.AddListener(PlaySound);
    }

    private void OnDestroy()
    {
        if (_targetButton == null || _audioDirector == null)
        {
            Log.InternalError();
            return;
        }
        
        _targetButton.onClick.RemoveListener(PlaySound);
    }

    private void PlaySound()
    {
        _audioDirector.PlayButtonClick();
    }

#if UNITY_EDITOR
    
    private void OnValidate()
    {
        if (_targetButton == null)
            _targetButton = GetComponent<Button>();
    }
    
#endif
}
}