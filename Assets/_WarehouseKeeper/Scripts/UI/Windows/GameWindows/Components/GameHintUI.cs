using DG.Tweening;
using UnityEngine;

namespace WarehouseKeeper._WarehouseKeeper.Scripts.UI.Windows.GameWindows.Components
{
public class GameHintUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup _selfCanvasGroup;
    [SerializeField] private Transform _root;
    [SerializeField] private Transform _live;
    [Header("Settings"),]
    [SerializeField] private float _decreaseDuration = 0.5f;
    [SerializeField] private float _liveScaleFactor = 1.3f;
    [SerializeField] private float _liveScaleDuration = 1f;

    private Tween _parentTween;
    private Tween _liveTween;

    public bool IsActive { get; private set; } = true;

    public void Enable()
    {
        StopTwins();
        IsActive = true;
        _root.localScale = Vector3.zero;
        _live.localScale = Vector3.one;
        _selfCanvasGroup.blocksRaycasts = false;

        _parentTween = _root.DOScale(1f, _decreaseDuration).OnComplete(() =>
        {
            _selfCanvasGroup.blocksRaycasts = true;
            _liveTween = _live.DOScale(Vector3.one * _liveScaleFactor, _liveScaleDuration).SetLoops(-1, LoopType.Yoyo);
        });
    }

    public void Disable()
    {
        StopTwins();
        IsActive = false;
        _selfCanvasGroup.blocksRaycasts = false;
        _live.DOScale(1f, _decreaseDuration);
        _root.DOScale(0f, _decreaseDuration);
    }

    public void TurnOn()
    {
        StopTwins();
        IsActive = true;
        _root.localScale = Vector3.one;
        gameObject.SetActive(true);
        _selfCanvasGroup.blocksRaycasts = true;
    }

    public void TurnOff()
    {
        StopTwins();
        IsActive = false;
        gameObject.SetActive(false);
        _selfCanvasGroup.blocksRaycasts = false;
    }

    private void StopTwins()
    {
        _parentTween?.Kill();
        _liveTween?.Kill();
    }

    private void OnDestroy()
    {
        StopTwins();
    }
    
}
}