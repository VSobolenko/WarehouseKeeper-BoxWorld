using UnityEngine;

namespace Game.GUI.Windows
{
[DefaultExecutionOrder(11), RequireComponent(typeof(CanvasGroup)), DisallowMultipleComponent]
public class WindowUI : MonoBehaviour
{
    [SerializeField] protected internal CanvasGroup canvasGroup;

    [ContextMenu("Validate")]
    private void Reset()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }
}
}