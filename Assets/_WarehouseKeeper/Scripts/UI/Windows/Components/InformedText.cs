using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace WarehouseKeeper.UI.Windows
{
public class InformedText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _sourceText;
    [SerializeField] private Vector3 _startPoint;

    [Header("Settings")] 
    [SerializeField] private float _duration = 2f;
    [Range(0f, 1f)]
    [SerializeField] private float _alphaStartToEndRatio = 0.3f;
    [Range(0f, 1f)]
    [SerializeField] private float _alphaDurationRatio = 0.5f;
    [Range(0f, 1f)]
    [SerializeField] private float _scaleLiftingRatio = 0.1f;
    [SerializeField] private float _liftingHeight = 25f;

    private Sequence _sequence;
    private Tween _fadeTween;
    private Tween _localMoveTween;
    
    public void Show(string text)
    {
        _sourceText.text = text;
        gameObject.SetActive(true);
        var selfTransform = transform;
        selfTransform.localScale = Vector3.zero;
        selfTransform.localPosition = _startPoint;
        KillAnimations();
        _sequence = DOTween.Sequence();

        _fadeTween = _sourceText.DOFade(1, _duration * _alphaStartToEndRatio / 2 / _alphaDurationRatio);
        _localMoveTween = selfTransform.DOLocalMoveY(_startPoint.y + _liftingHeight, _duration);
        _sequence.Append(selfTransform.DOScale(Vector3.one, _duration * _scaleLiftingRatio));
        _sequence.AppendInterval(1.5f);
        _sequence.Append(_sourceText.DOFade(0, _duration * (1 - _alphaStartToEndRatio) / 2 / _alphaDurationRatio));
        _sequence.AppendCallback(() => gameObject.SetActive(false));
        _sequence.Play();
    }

    private void KillAnimations()
    {
        _sequence?.Kill();
        _fadeTween?.Kill();
        _localMoveTween?.Kill();
    }
    
    private void OnDestroy()
    {
        KillAnimations();
    }

    private void OnValidate()
    {
        if (_sourceText == null)
            _sourceText = GetComponentInChildren<TextMeshProUGUI>();
    }
}
}