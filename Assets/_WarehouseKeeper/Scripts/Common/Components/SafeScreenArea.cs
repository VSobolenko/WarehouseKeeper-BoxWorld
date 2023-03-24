using System;
using UnityEngine;

namespace WarehouseKeeper.Components
{
[RequireComponent(typeof(RectTransform))]
public class SafeScreenArea : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransformComponent;
    
    private void Awake()
    {
        RecalculateRectTransform();
    }

    public void RecalculateRectTransform()
    {
#if UNITY_EDITOR
        if (rectTransformComponent == null)
            throw new ArgumentNullException(nameof(rectTransformComponent), "Rect Transform is null");
#endif
        
        var safeArea = Screen.safeArea;
        var anchorMin = safeArea.position;
        var anchorMax = anchorMin + safeArea.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;
        rectTransformComponent.anchorMin = anchorMin;
        rectTransformComponent.anchorMax = anchorMax;
    }
     
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (rectTransformComponent == null)
            rectTransformComponent = GetComponent<RectTransform>();
    }
    
    private void Reset()
    {
        OnValidate();
    }
#endif
}
}