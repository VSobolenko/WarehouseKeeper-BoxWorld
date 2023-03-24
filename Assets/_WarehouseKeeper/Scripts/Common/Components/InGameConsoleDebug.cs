using System;
using UnityEngine;
using WarehouseKeeper.Extension;

namespace WarehouseKeeper.Components
{
/// <summary>
/// In game debug console to view console from build
/// Make sure that resources asset not include to release build
/// Move file outside from resources folder
/// </summary>
public class InGameConsoleDebug : MonoBehaviour
{
#if DEVELOPMENT_BUILD
    [SerializeField] private bool _enableInEditor;
    [SerializeField] private int _countTouchToDestroy = 5;

    private GameObject _cachedConsole;
    
    private void Start()
    {
        EnableInGameDebugConsole();
    }
    
    private void EnableInGameDebugConsole()
    {
        
#if UNITY_EDITOR
        
        if (_enableInEditor == false)
            return;
        
#endif
        
        var consolePrefab = Resources.Load<GameObject>("IngameDebugConsole");
        if (consolePrefab == null) 
        {
            Log.WriteWarning("IngameDebugConsolePrefab not found");
            return;
        }

        _cachedConsole = Instantiate(consolePrefab);
    }

    private void Update()
    {
        if (Input.touchCount == _countTouchToDestroy)
            Destroy(_cachedConsole.gameObject);
    }

#endif

#if UNITY_EDITOR

    private const string InResourcesFolder = "Assets/Plugins/IngameDebugConsole/Resources/IngameDebugConsole.prefab";
    private const string OutsideResourcesFolder = "Assets/Plugins/IngameDebugConsole/IngameDebugConsole.prefab";
    
    [ContextMenu("Select prefab asset")]
    private void SelectInGamePrefab()
    {
        GameObject inGameConsole;
        inGameConsole = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(InResourcesFolder);
        if (inGameConsole == null)
            inGameConsole = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(OutsideResourcesFolder);

        if (inGameConsole == null)
        {
            Debug.LogWarning("Can't find asset");
            return;
        }

        UnityEditor.Selection.activeObject = inGameConsole;
    }
    
    [ContextMenu("Move to resource folder")]
    private void MoveToResources()
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
    
    [ContextMenu("Move from resource folder")]
    private void MoveFromResources()
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
    
#endif
}
}