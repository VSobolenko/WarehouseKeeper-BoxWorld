using Game.Installers.IO;
using Game.Installers.Repositories;
using Game.Repositories;
using UnityEditor;
using UnityEngine;
using WarehouseKeeper.EditorScripts;
using WarehouseKeeper.Levels;
using Zenject;

namespace WarehouseKeeper.EditorTools.Levels
{
public class EditorLevelSettingsGenerator : EditorWindow
{
    private LevelSettingsGenerator _levelGenerator;
    private const string PathToLevels = "_WarehouseKeeper/DynamicAssets/Resources/Levels/";
    private string _console = "Level generator console";

    [MenuItem(EditorGameData.EditorName + "/Level generator")]
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
        
        var container = new DiContainer();
        EditorFileIOInstaller.Install(container);
        EditorRepositoryInstaller<LevelSettings>.Install(container, savePath);

        var repository = container.Resolve<IRepository<LevelSettings>>();
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