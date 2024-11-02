using Game;
using Game.Components.EditorComponent;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace WarehouseKeeper.EditorScripts.ToolsHelper
{
[CustomEditor(typeof(RaycastBypassEditorUI))]
public class RaycastBypassEditorTools : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Disable graphic raycast in children") && target is RaycastBypassEditorUI raycastGraphic)
            DisableRaycast<Graphic>(raycastGraphic.gameObject);
        GUILayout.Space(10);
        
        if (GUILayout.Button("Disable images raycast in children") && target is RaycastBypassEditorUI raycastImage)
            DisableRaycast<Image>(raycastImage.gameObject);
        GUILayout.Space(10);
        
        if (GUILayout.Button("Disable TextMeshProUGUI raycast in children") && target is RaycastBypassEditorUI raycastTextMeshProUGUI)
            DisableRaycast<TextMeshProUGUI>(raycastTextMeshProUGUI.gameObject);
        GUILayout.Space(10);
    }
    
    private void DisableRaycast<T>(GameObject assignedGameObject) where T : Graphic
    {
        var raycastComponents = assignedGameObject.GetComponentsInChildren<T>(true);
        if (raycastComponents == null)
        {
            Log.Info($"Objects for type {typeof(T)} not found");
            return;
        }

        var countDisable = 0;
        foreach (var component in raycastComponents)
        {
            if (component.raycastTarget)
                countDisable++;
            component.raycastTarget = false;
        }
        
        Log.Info($"Disable raycast: CountObjects={raycastComponents.Length}; CountDisabled={countDisable}");
        EditorUtility.SetDirty(this);
    }
}
}