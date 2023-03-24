using UnityEditor;
using UnityEngine;
using WarehouseKeeper.EditorScripts;
using WarehouseKeeper.Levels;
using WarehouseKeeper.Repositories;
using WarehouseKeeper.Repositories.FileSave;
using WarehouseKeeper.Repositories.FileSave.Implementation;
using WarehouseKeeper.Repositories.Implementation;

namespace WarehouseKeeper.EditorTools.Levels
{
public class EditorLevelSettingsGenerator : EditorWindow
{
    private LevelSettingsGenerator _levelGenerator;
    private const string PathToLevels = "_WarehouseKeeper/DynamicAssets/Resources/Levels/";
    private string _console = "Level generator console";

    [MenuItem(EditorGameNaming.WindowStartedName + "Level generator")]
    private static void ShowWindow()
    {
        var window = GetWindow<EditorLevelSettingsGenerator>();
        window.titleContent = new GUIContent("Levels");
        window.Show();
    }

    private void OnGUI()
    {
        if (_levelGenerator == null)
            SetupGenerator();

        _levelGenerator?.DrawGUI();
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear console", GUILayout.MaxWidth(100)))
            _console = "";
        EditorGUILayout.LabelField(_console);
        if (GUILayout.Button("Path", GUILayout.MaxWidth(50)))
            _console = PathToLevels;
        EditorGUILayout.EndHorizontal();
    }

    private void SetupGenerator()
    {
        var savePath = $"{Application.dataPath}/{PathToLevels}";
        var defaultGridSize = new Vector2Int(3, 3);
        ISaveFile saveFile = new BinarySave();
        IRepository<LevelSettings> repository = new FileRepositoryManager<LevelSettings>(savePath, saveFile, false);
        _levelGenerator = new LevelSettingsGenerator(repository, OnLogText, defaultGridSize, ReimportResourcesLevels);
    }

    private void OnLogText(string text)
    {
        _console = text;
    }

    private void ReimportResourcesLevels()
    {
        AssetDatabase.Refresh();
    }
}
}