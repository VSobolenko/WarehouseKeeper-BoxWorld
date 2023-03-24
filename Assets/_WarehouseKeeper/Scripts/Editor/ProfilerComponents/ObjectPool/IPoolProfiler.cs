using System;
using UnityEngine.UIElements;

namespace WarehouseKeeper.EditorScripts.ObjectPool
{
public interface IPoolProfiler
{
    void DrawStatus(VisualElement root);

    void OnPoolDataUpdated();
}
}