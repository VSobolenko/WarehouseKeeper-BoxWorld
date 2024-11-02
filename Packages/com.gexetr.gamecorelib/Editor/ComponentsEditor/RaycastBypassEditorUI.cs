#if UNITY_EDITOR
using UnityEngine;

namespace Game.Components.EditorComponent
{
/// <summary>
/// Class which provide inspector functionality in runtime
/// </summary>
internal class RaycastBypassEditorUI : MonoBehaviour
{
    private void OnEnable()
    {
        Log.Warning($"Editor only {GetType().Name} component. Removed this from {name} gameObject");
        Destroy(this);
    }
}
}
#endif