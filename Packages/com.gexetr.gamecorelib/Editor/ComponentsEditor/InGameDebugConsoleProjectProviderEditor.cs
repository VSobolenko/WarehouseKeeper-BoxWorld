using Game.InternalData;
using UnityEngine;

namespace Game.Components.EditorComponent
{
public static class InGameDebugConsoleProjectProviderEditor
{
    private const string InResourcesFolder = "Assets/Plugins/IngameDebugConsole/Resources/IngameDebugConsole.prefab";
    private const string OutsideResourcesFolder = "Assets/Plugins/IngameDebugConsole/IngameDebugConsole.prefab";
    
    private const string Title = "In-game console";
    
    private const string InspectAsset = "Inspect prefab asset";
    private const string ToResources = "Move to resource folder";
    private const string FromResources = "Move from resource folder";
    
    [UnityEditor.MenuItem(GameData.EditorName + "/" + Title + "/" + InspectAsset)]
    private static void SelectInGamePrefabStatic()
    {
        var inGameConsole = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(InResourcesFolder);
        if (inGameConsole == null)
            inGameConsole = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(OutsideResourcesFolder);

        if (inGameConsole == null)
        {
            Debug.LogWarning("Can't find asset");
            return;
        }

        UnityEditor.Selection.activeObject = inGameConsole;
    }
    
    [UnityEditor.MenuItem(GameData.EditorName + "/" + Title + "/" + ToResources)]
    private static void MoveToResourcesStatic()
    {
        var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(OutsideResourcesFolder);
        if (asset == null)
        {
            Debug.LogWarning($"Asset from [{OutsideResourcesFolder}] not found");
            return;
        }

        var result = UnityEditor.AssetDatabase.MoveAsset(OutsideResourcesFolder, InResourcesFolder);
        Debug.Log($"Result moving: {(string.IsNullOrEmpty(result) ? "Success" : result)}");
        
        UnityEditor.AssetDatabase.Refresh();
    }
    
    [UnityEditor.MenuItem(GameData.EditorName + "/" + Title + "/" + FromResources)]
    private static void MoveFromResourcesStatic()
    {
        var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(InResourcesFolder);
        if (asset == null)
        {
            Debug.LogWarning($"Asset from [{InResourcesFolder}] not found");
            return;
        }

        var result = UnityEditor.AssetDatabase.MoveAsset(InResourcesFolder, OutsideResourcesFolder);
        Debug.Log($"Result moving: {(string.IsNullOrEmpty(result) ? "Success" : result)}");
        
        UnityEditor.AssetDatabase.Refresh();
    }
}
}