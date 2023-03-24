using System.Threading;
using DG.Tweening;
using UnityEngine;

namespace WarehouseKeeper.Levels.Entities
{
internal class PlayerEntity : EntityGame
{
    [SerializeField] private float _rotationSpeed = 17f;
    [SerializeField] private Animator _viewAnimator;
    [SerializeField] private string _animKey;
    [SerializeField] private Transform _view;
    
    private Tween _moveTween;
    private Vector3 _targetPosition;
    
    #region Moving

    public override void StartViewMoving(Vector3 targetPosition, float duration, CancellationToken token)
    {
        _targetPosition = (targetPosition - transform.position).normalized;

        _moveTween?.Kill();
        _moveTween = transform.DOMove(targetPosition, duration).OnUpdate(() =>
        {
            if (token.IsCancellationRequested)
                _moveTween?.Kill();
        });
    }

    public override void StopViewMoving(Vector3 confirmPosition)
    {
        _moveTween?.Kill();
        _targetPosition = confirmPosition;
        transform.position = confirmPosition;
    }

    private void Update()
    {
        var epsilon = transform.position - _targetPosition;
        if (epsilon.magnitude < 0.001f) 
            return;
        
        var direction = Quaternion.LookRotation(_targetPosition);
        _view.rotation = Quaternion.Lerp(_view.rotation, direction, _rotationSpeed * Time.deltaTime);
    }

    #endregion

    #region View states

    public override void UpdateState(Node node)
    {
        _viewAnimator.SetInteger(_animKey, 0);
        _targetPosition = transform.position;
    }
    
    public override void OnStartMoving(Vector2Int direction, GameEntity self, Node[,] nodes, GameEntity[,] gameEntities)
    {
        var targetNode = nodes[self.X + direction.x, self.Y + direction.y];
        var entityOnNode = gameEntities[self.X + direction.x, self.Y + direction.y];
        var animationValue = entityOnNode.Type == GameEntityType.Box ? 2 : 1;
        _viewAnimator.SetInteger(_animKey, animationValue);
    }

    public override void OnEndMoving()
    {
        _viewAnimator.SetInteger(_animKey, 0);
    }

    #endregion

    public override void OnRelease()
    {
        _moveTween?.Kill();
    }
}
}