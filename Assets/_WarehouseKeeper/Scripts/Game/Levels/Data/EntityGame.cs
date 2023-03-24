using System.Threading;
using UnityEngine;
using WarehouseKeeper.Pools;

namespace WarehouseKeeper.Levels
{
internal class EntityGame : BasePooledObject
{
    public virtual void UpdateState(Node node) { }
    public virtual void OnStartMoving(Vector2Int direction, GameEntity self, Node[,] nodes, GameEntity[,] gameEntities) { }
    public virtual void OnEndMoving() { }

    public virtual void StartViewMoving(Vector3 targetPosition, float duration, CancellationToken token)
    {
        transform.position = targetPosition;
    }

    public virtual void StopViewMoving(Vector3 confirmPosition)
    {
        transform.position = confirmPosition;
    }
    
}
}