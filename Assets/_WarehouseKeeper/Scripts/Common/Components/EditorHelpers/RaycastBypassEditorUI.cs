using UnityEngine;
using WarehouseKeeper.Extension;

namespace WarehouseKeeper.Components.EditorHelpers
{
public class RaycastBypassEditorUI : MonoBehaviour
{
    private void OnEnable()
    {
        Log.WriteWarning($"Editor only {GetType().Name} component. Removed this from {name} gameObject");
        Destroy(this);
    }
}
}