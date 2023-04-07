using UnityEditor;
using UnityEngine;

namespace WarehouseKeeper.EditorScripts
{
public class TestEditorWindow : EditorWindow
{
    [MenuItem(EditorGameData.EditorName + "/Test window")]
    private static void ShowWindow()
    {
        var window = GetWindow<TestEditorWindow>();
        window.titleContent = new GUIContent("Test Window");
        window.Show();
    }

}
}