using System.Collections.Generic;
using UnityEngine;

namespace WarehouseKeeper.EditorTools.Levels
{
public class HintTunel : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private bool _receiveHints = true;
    [SerializeField] private List<Vector2Int> _hintTunel;

    public void AddDirection(Vector2Int direction)
    {
        if (_receiveHints == false)
            return;
        
        _hintTunel.Add(direction);
    }
    
    public void RemoveLast()
    {
        if (_hintTunel.Count <= 0)
            return;
        _hintTunel.RemoveAt(_hintTunel.Count - 1);
    }

    public List<Vector2Int> GetDirections()
    {
        return new List<Vector2Int>(_hintTunel);
    }
    public void ClearHints() => _hintTunel.Clear();
#endif
}
}