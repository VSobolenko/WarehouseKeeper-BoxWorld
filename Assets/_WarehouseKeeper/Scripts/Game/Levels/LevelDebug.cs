using UnityEditor;
using UnityEngine;
using Zenject;

namespace WarehouseKeeper.Levels
{
public class LevelDebug : MonoBehaviour
{
#if UNITY_EDITOR
    [Inject] private LevelDirector _levelDirector;

    [SerializeField] private bool _enableNodeDebug;
    [SerializeField] private bool _enableEntityDebug;
    [Space, SerializeField] private bool _enableType;

    private void OnDrawGizmos()
    {
        var activeLevel = _levelDirector?.ActiveLevel;
        if (activeLevel == null)
            return;

        if (_enableNodeDebug)
        {
            DrawNodeDebug(activeLevel);
        }

        if (_enableEntityDebug)
        {
            DrawEntityDebug(activeLevel);
        }
    }

    private void DrawNodeDebug(GameLevel activeLevel)
    {
        for (int x = 0; x < activeLevel.nodes.GetLength(0); x++)
        {
            for (int y = 0; y < activeLevel.nodes.GetLength(1); y++)
            {
                var node = activeLevel.nodes[x, y];
                if (node.Type == PieceType.Empty)
                    continue;

                var data = _enableType ? $"[{node.Type}][{x}, {y}]" : $"[{x}, {y}]";
                Handles.Label(node.Entity.transform.position + Vector3.up / 2f + Vector3.right / 2f, data);
            }
        }
    }

    private void DrawEntityDebug(GameLevel activeLevel)
    {
        for (int x = 0; x < activeLevel.entities.GetLength(0); x++)
        {
            for (int y = 0; y < activeLevel.entities.GetLength(1); y++)
            {
                var entity = activeLevel.entities[x, y];
                if (entity.Type == GameEntityType.None)
                    continue;
                var data = _enableType ? $"[{entity.Type}][{x}, {y}]" : $"[{x}, {y}]";
                Handles.Label(entity.Entity.transform.position + Vector3.up / 2f + Vector3.right / 2f, data);
            }
        }
    }
#endif
}
}