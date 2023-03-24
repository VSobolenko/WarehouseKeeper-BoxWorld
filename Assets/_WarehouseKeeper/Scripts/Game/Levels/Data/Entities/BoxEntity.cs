using System.Threading;
using DG.Tweening;
using UnityEngine;
using WarehouseKeeper.Extension;

namespace WarehouseKeeper.Levels.Entities
{
internal class BoxEntity : EntityGame
{
    [SerializeField] private Material _inactive;
    [SerializeField] private Material _active;
    [SerializeField] private MeshRenderer _meshRenderer;

    private Node _finishUpdateNode;

    #region Moving

    private Tween _moveTween;
    
    public override void StartViewMoving(Vector3 targetPosition, float duration, CancellationToken token)
    {
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
        transform.position = confirmPosition;
    }

    #endregion
    
    #region View states

    public override void UpdateState(Node node)
    {
        if (node == null) 
        {
            Log.InternalError();
            return;
        }

        var targetMaterial = _inactive;
        if (node.Type == PieceType.Target)
            targetMaterial = _active;
        
        _meshRenderer.material = targetMaterial;
    }

    public override void OnStartMoving(Vector2Int direction, GameEntity self, Node[,] nodes, GameEntity[,] gameEntities)
    {
        _finishUpdateNode = null;
        
        var targetNode = nodes[self.X + direction.x, self.Y + direction.y];
        if (targetNode.Type == PieceType.PlayZone)
            UpdateState(targetNode);
        else
            _finishUpdateNode = targetNode;
    }

    public override void OnEndMoving()
    {
        if (_finishUpdateNode != null)
            UpdateState(_finishUpdateNode);
    }

    #endregion

    public override void OnRelease()
    {
        _finishUpdateNode = null;
        _moveTween?.Kill();
    }
}
}