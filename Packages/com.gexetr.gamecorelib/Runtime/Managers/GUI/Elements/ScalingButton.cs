using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.GUI
{
[DisallowMultipleComponent]
public class ScalingButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float normalScale = 1f;
    [SerializeField] private float downScale = 0.96f;
    [SerializeField] private float upScale = 1.04f;

    private Tween _scaleTween;
        
    public void OnPointerDown(PointerEventData eventData)
    {
        _scaleTween?.Kill();
        _scaleTween = transform.DOScale(downScale, 0.04f).SetEase(Ease.InOutCubic);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _scaleTween?.Kill();
        _scaleTween = transform.DOScale(upScale, 0.08f).SetEase(Ease.InOutCubic).OnComplete(() =>
        {
            _scaleTween?.Kill();
            _scaleTween = transform.DOScale(normalScale, 0.06f).SetEase(Ease.InOutCubic);
        });
    }

    private void OnDestroy() => _scaleTween?.Kill();
}
}