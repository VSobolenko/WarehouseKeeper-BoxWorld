using DG.Tweening;
using Game.Pools;
using UnityEngine;

namespace WarehouseKeeper.Directors.Game.Hints
{
internal class DirectionalArrow : BasePooledObject
{
    [SerializeField] private Transform _arrow;
    [SerializeField] private float _distance = 2f ;
    [SerializeField] private float _moveDuration = 0.8f ;
    [SerializeField] private float _fadeDuration = 0.3f ;
    [SerializeField] private float _repeatableDelay = 0.2f ;

    private Sequence _activeSequence;
    private Tween _fadeTween;
    
    public void SetStartPosition(Vector3 position)
    {
        StopTweens();
        transform.position = position;
        _arrow.localScale = Vector3.zero;
    }

    public void Enable(Vector2 direction)
    {
        StopTweens();
        var worldDirection = new Vector3(direction.x, 0, direction.y);
        _arrow.forward = worldDirection;
        _arrow.localScale = Vector3.zero;
        _arrow.localPosition = Vector3.zero;
        var firstStep = worldDirection * _distance / 3f;
        var secondStep = firstStep + worldDirection * _distance / 3f;
        var thirdStep = worldDirection * _distance;
        
        _activeSequence = DOTween.Sequence();
        _activeSequence.Append(_arrow.DOLocalMove(firstStep, _moveDuration / 3f).SetEase(Ease.Linear));
        _activeSequence.Join(_arrow.DOScale(1f, Mathf.Min(_fadeDuration, _moveDuration / 2f)).SetEase(Ease.Linear));

        _activeSequence.Append(_arrow.DOLocalMove(secondStep, _moveDuration / 3f).SetEase(Ease.Linear));

        _activeSequence.Append(_arrow.DOLocalMove(thirdStep, _moveDuration / 3f).SetEase(Ease.Linear));
        _activeSequence.Join(_arrow.DOScale(0f, Mathf.Min(_fadeDuration, _moveDuration / 2f)).SetEase(Ease.Linear));

        _activeSequence.AppendInterval(_repeatableDelay);
        _activeSequence.SetLoops(-1, LoopType.Restart);
    }

    public void Disable()
    {
        _fadeTween = _arrow.DOScale(0f, Mathf.Min(_fadeDuration, _moveDuration / 2f)).OnComplete(StopTweens);
    }

    private void StopTweens()
    {
        _fadeTween?.Kill();
        _activeSequence?.Kill();
    }

    private void OnDestroy()
    {
        StopTweens();
    }
    
#if UNITY_EDITOR

    [Space, SerializeField] private Vector2 _debugDirection;
    
    [ContextMenu("Editor enable forward moving")]
    private void EditorEnable()
    {
        Enable(_debugDirection);
    }
    
    [ContextMenu("Editor disable")]
    private void EditorDisable()
    {
        Disable();
    }
#endif
}
}