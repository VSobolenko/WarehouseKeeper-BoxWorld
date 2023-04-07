using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Game;
using UnityEngine;

namespace WarehouseKeeper.UI.Windows.GameWindows.Components
{
public class GameWindowFadeTransition : MonoBehaviour
{
    [SerializeField] private CanvasGroup _targetGroup;
    [Space, Range(0f, 1f), SerializeField] private float _enableAlpha = 1f;
    [SerializeField] private float _enableDuration = 0.3f;
    [Range(0f, 1f), SerializeField] private float _disableAlpha = 0.7f;
    [SerializeField] private float _disableDuration = 0.3f;

    private TweenerCore<float, float, FloatOptions> _fadeTween;
    
    public void Enable()
    {
        if (_targetGroup == null)
        {
            Log.WriteWarning($"Null target canvas group in {name}");
            return;
        }
        _targetGroup.blocksRaycasts = true;
        FadeTargetGroup(_enableAlpha, _enableDuration);
    }
    
    public void Disable()
    {
        if (_targetGroup == null)
        {
            Log.WriteWarning($"Null target canvas group in {name}");
            return;
        }
        
        _targetGroup.blocksRaycasts = false;
        FadeTargetGroup(_disableAlpha, _disableDuration);
    }

    private void FadeTargetGroup(float targetValue, float targetDuration)
    {
        _fadeTween?.Kill();
        
        /* Liner interpolation if previous tween not finished
        var alpha = _targetGroup.alpha;
        var expectedStartedValue = 1f;
        if (_fadeTween != null)
            expectedStartedValue = _fadeTween.endValue;

        //var durationTime = Mathf.Lerp(0, targetDuration, 1 - (alpha - targetValue) / alpha);
        //var durationTime = Mathf.Lerp(0, targetDuration, 1 - (targetValue - expectedStartedValue) / (alpha - expectedStartedValue - targetValue));
        */

        _fadeTween = _targetGroup.DOFade(targetValue, targetDuration);
    }

    private void OnDestroy()
    {
        _fadeTween?.Kill();
    }
    
#if UNITY_EDITOR

    private void OnValidate()
    {
        if (_targetGroup == null)
            _targetGroup = GetComponent<CanvasGroup>();
    }
    
#endif
}
}