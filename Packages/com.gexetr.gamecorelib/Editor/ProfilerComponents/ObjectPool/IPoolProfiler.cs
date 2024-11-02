using System;
using UnityEngine.UIElements;

namespace GameEditor.Pools
{
public interface IPoolProfiler
{
    void DrawStatus(VisualElement root);

    void OnPoolDataUpdated();
}
}