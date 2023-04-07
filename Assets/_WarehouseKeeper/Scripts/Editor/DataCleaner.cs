using System.IO;
using UnityEditor;
using UnityEngine;

namespace WarehouseKeeper.EditorScripts
{
public class DataCleaner : EditorWindow
{
    private string PathToUserData => $"{Application.persistentDataPath}/UserData";
    
    [MenuItem(EditorGameData.EditorName + "/Data manager", false)]
    private static void ShowWindow()
    {
        var window = GetWindow<DataCleaner>();
        window.titleContent = new GUIContent("Data manager");
        window.Show();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Clear User data"))
            ClearUserData();
        GUILayout.Space(20);
        if (GUILayout.Button("Clear Saved data"))
            ClearSavedData();
        GUILayout.Space(5);
        if (GUILayout.Button("Clear Player prefs"))
            ClearPlayerPrefs();

        GUILayout.Label($"Runtime path to save: {PathToUserData}");
        GUILayout.Label($"Levels path: _WarehouseKeeper/DynamicAssets/Resources/Levels/");
    }

    private void ClearUserData()
    {
        PlayerPrefs.DeleteAll();
        if (Directory.Exists(PathToUserData))
            Directory.Delete(PathToUserData, true);

        Debug.Log("User data cleared!");
    }
    
    private void ClearSavedData()
    {
        if (Directory.Exists(PathToUserData))
            Directory.Delete(PathToUserData, true);
        
        Debug.Log("Saved data cleared!");
    }
    
    private void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();

        Debug.Log("Player prefs cleared!");
    }

}
}