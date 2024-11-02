using System;
using System.Collections;
using Game.Pools;
using UnityEngine;

namespace Game.Audio
{
[RequireComponent(typeof(AudioSource))]
public class Source : KeyPooledObject
{
    [field: SerializeField] public AudioSource AudioSource { get; private set; }
    
    private Coroutine _coroutine;
    private Coroutine _autoAction;

    public bool IsFadeProcessActive => _coroutine != null;

    public void StopSource(Action completeAction = null)
    {
        if (StopActiveFaded())
            Log.Warning($"A new fading process is started on the active object; Name={name};ActiveSound={AudioSource.clip}");
        AudioSource.Stop();
        completeAction?.Invoke();
    }
    
    public void FadeSource(float duration, float targetVolume = 1, Action completeAction = null)
    {
        if (StopActiveFaded())
            Log.Warning($"A new fading process is started on the active object; Name={name};ActiveSound={AudioSource.clip}");

        _coroutine = StartCoroutine(FadeSound(duration, targetVolume, () =>
        {
            StopActiveFaded();
            completeAction?.Invoke();
        }));
    }

    public void EnableAutoAction(float duration, Action action)
    {
        _autoAction = StartCoroutine(AutoAction(duration, action));
    }
    
    private IEnumerator AutoAction(float duration, Action completeAction = null)
    {
        //yield return new WaitForSeconds(duration);
        completeAction?.Invoke();
        _autoAction = null;
        yield return new WaitForSeconds(duration);

    }
    
    private IEnumerator FadeSound(float duration, float targetVolume, Action completeAction = null)
    {
        var progress = 0.0f;
        var timePassed = 0.0f;
        var originVolume = AudioSource.volume;
        targetVolume = Mathf.Clamp01(targetVolume);

        while(progress < 1)
        {
            progress = timePassed / duration;

            AudioSource.volume = Mathf.Lerp(originVolume, targetVolume, progress);

            timePassed += Time.deltaTime;

            yield return null;
        }
        completeAction?.Invoke();
    }

    private bool StopActiveFaded()
    {
        if (_coroutine ==null)
            return false;
        
        StopCoroutine(_coroutine);
        _coroutine = null;
        return true;
    }
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (AudioSource == null)
        {
            AudioSource = GetComponent<AudioSource>();
        }
    }
    
#endif
}
}