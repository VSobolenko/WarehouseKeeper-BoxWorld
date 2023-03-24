using UnityEditor;
using UnityEngine;

namespace WarehouseKeeper.EditorScripts
{
public class TestEditorWindow : EditorWindow
{
    [MenuItem("Warehouse Keeper/Test window")]
    private static void ShowWindow()
    {
        var window = GetWindow<TestEditorWindow>();
        window.titleContent = new GUIContent("Test Window");
        window.Show();
    }

}
}