using System;
using UnityEditor;
using UnityEngine;

namespace WarehouseKeeper.EditorScripts
{
public class EditorWindowTemplate : EditorWindow
{
    [MenuItem(EditorGameData.EditorName + "/Editor template")]
    public static void OpenWindow()
    {
        var window = GetWindow<EditorWindowTemplate>();
        window.titleContent = new GUIContent("Window name");
        Debug.Log("Called OpenWindow method");
    }

    private void CreateGUI()
    {
    }

    private void OnGUI()
    {
    }
}
}