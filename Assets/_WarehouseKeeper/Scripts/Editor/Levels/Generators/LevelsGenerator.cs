using System;
using System.Linq;
using Game.Repositories;
using UnityEditor;
using UnityEngine;

namespace WarehouseKeeper.EditorTools.Levels
{
internal abstract class LevelsGenerator<TData, TLevel>
    where TData : class, IHasBasicId, new()
    where TLevel : struct
{
    protected readonly IRepository<TData> repository;
    private readonly Action _updateAssets;
    private readonly Vector2Int _defaultGridSize;
    private readonly Vector2 _offset;
    private readonly Action<string> _log;
    private int _editorGridSizeX = 3;
    private int _editorGridSizeY = 3;
    private Vector2 _scrollPos;

    protected LevelsGenerator(IRepository<TData> repository, Action<string> log, Vector2Int defaultGridSize, Action onUpdateAssets)
    {
        this.repository = repository;
        _defaultGridSize = defaultGridSize;
        this._log = log;
        _updateAssets = onUpdateAssets;
        
        _offset = new Vector2(16f, 2f);
        _levels = new TLevel[0, 0];
        
        RebuildGameGrid(_defaultGridSize, false);
    }

    private int _levelId;

    private TLevel[,] _levels;

    public void DrawGUI()
    {
        DrawManagePanel();
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, false, false, GUI.skin.horizontalScrollbar,
                                                    GUI.skin.verticalScrollbar, GUI.skin.box);
        DrawGameGrid();

        EditorGUILayout.EndScrollView();
        GUILayout.FlexibleSpace();

        DrawUserPanel();
        DrawLevelCrud();
    }

    private void DrawManagePanel()
    {
        var warningStyle = new GUIStyle(GUI.skin.button)
        {
            normal = {textColor = Color.red},
        };

        var safeStyle = new GUIStyle(GUI.skin.button)
        {
            normal = {textColor = Color.green},
        };

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Reset window", safeStyle))
        {
            ResetUserPanel();
            RebuildGameGrid(_defaultGridSize, false);
            _log?.Invoke("Reset window settings");
        }

        if (GUILayout.Button("Reset Active grid", warningStyle))
        {
            RebuildGameGrid(false);
            _log?.Invoke($"Reset active grid");
        }

        EditorGUILayout.EndHorizontal();
    }

    /*
     * 1. ----------------------------------------------------------------------------------+
     * for (var x = 0; x < _levels.GetLength(0); x++)             x | (0,0)  (0,1)  (0,2)   |
     * {                                                            | (1,0)  (1,1)  (1,2)   |
     *     for (var y = 0; y < _levels.GetLength(1); y++)           | (2,0)  (2,1)  (2,2)   |
     *     {                                                        | (3,0)  (3,1)  (3,2)   |
     *     }                                                        + ----------------->    |
     * }                                                                               y    |
     *--------------------------------------------------------------------------------------+
     * 2. -------------------------------------------------------------------------------
     * for (var x = _levels.GetLength(0) - 1; x >= 0; x--)        x | (3,0)  (3,1)  (3,2)
     * {                                                            | (2,0)  (2,1)  (2,2)
     *     for (var y = 0; y < _levels.GetLength(1); y++)           | (1,0)  (1,1)  (1,2)
     *     {                                                        | (0,0)  (0,1)  (0,2)
     *     }                                                        + ----------------->
     * }                                                                               y
     *
     * 3. -------------------------------------------------------------------------------
     * for (var x = 0; x < _levels.GetLength(0); x++)             x | (3,2)  (3,1)  (3,0)
     * {                                                            | (2,2)  (2,1)  (2,0)
     *     for (var y = _levels.GetLength(1) - 1; y >= 0; y--)      | (1,2)  (1,1)  (1,0)
     *     {                                                        | (0,2)  (0,1)  (0,0)
     *     }                                                        + ----------------->
     * }                                                                               y
     *
     * 4. -------------------------------------------------------------------------------
     * for (var y = 0; y < _levels.GetLength(1); y++)             x | (0,0)  (1,0)  (2,0)
     * {                                                            | (0,1)  (1,1)  (2,1)
     *     for (var x = 0; x < _levels.GetLength(0); x++)           | (0,2)  (1,2)  (2,2)
     *     {                                                        | (0,3)  (1,3)  (2,3)
     *     }                                                        + ----------------->
     * }                                                                               y
     *
     * 5. ------------------------------------------------------------------------------- - Current
     * for (var y = 0; y < _levels.GetLength(1); y++)             x | (0,3)  (1,3)  (2,3)
     * {                                                            | (0,2)  (1,2)  (2,2)
     *     for (var x = 0; x < _levels.GetLength(0); x++)           | (0,1)  (1,1)  (2,1)
     *     {                                                        | (0,0)  (1,0)  (2,0)
     *     }                                                        + ----------------->
     * }                                                                               y
     * 
     */
    private void DrawGameGrid()
    {
        for (var y = _levels.GetLength(1) - 1; y >= 0; y--)
        {
            EditorGUILayout.BeginHorizontal();
            for (var x = 0; x < _levels.GetLength(0); x++)
            {
                DrawLevelPart(ref _levels[x, y], out var countVerticalElements);

                if (x == _levels.GetLength(0) - 1)
                    continue;
                
                EditorGUILayout.Space(_offset.x / 2);
                EditorGUILayout.BeginVertical();

                for (var i = 0; i < countVerticalElements; i++)
                {
                    EditorGUILayout.LabelField("", GUI.skin.verticalSlider, GUILayout.MaxWidth(5));
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(_offset.x / 2);
            }

            EditorGUILayout.EndHorizontal();

            if (y == 0)
                continue;
            EditorGUILayout.Space(_offset.y / 2);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space(_offset.y / 2);

        }
    }

    protected abstract void DrawLevelPart(ref TLevel partData, out int countVerticalElements);
    
    private void DrawUserPanel()
    {
        DrawCustomUserPanel();

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Level ID = ", GUILayout.MaxWidth(80));
        _levelId = EditorGUILayout.IntField(_levelId, GUILayout.MaxWidth(150));

        GUILayout.Space(5);
        EditorGUILayout.LabelField("Grid size:", GUILayout.MaxWidth(70));
        EditorGUILayout.LabelField("X", GUILayout.MaxWidth(10));
        _editorGridSizeX = EditorGUILayout.IntField(_editorGridSizeX, GUILayout.MaxWidth(100));
        EditorGUILayout.LabelField("Y", GUILayout.MaxWidth(10));
        _editorGridSizeY = EditorGUILayout.IntField(_editorGridSizeY, GUILayout.MaxWidth(100));
        
        
        if (GUILayout.Button("Rebuild", GUILayout.MaxWidth(100)))
            RebuildGameGrid(new Vector2Int(_editorGridSizeX, _editorGridSizeY), true);
        
        EditorGUILayout.EndHorizontal();

    }

    protected virtual void DrawCustomUserPanel() { }

    private void DrawLevelCrud()
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("SAVE"))
            if (TrySaveLevel(_levelId))
                _log?.Invoke($"Levels saved! Id={_levelId}");

        EditorGUILayout.BeginVertical();

        if (GUILayout.Button("LOAD"))
            if (TryLoadLevel(_levelId))
                _log?.Invoke($"Levels load! Id={_levelId}");

        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical();

        if (GUILayout.Button("UPDATE"))
            if (TryUpdateLevel(_levelId))
                _log?.Invoke($"Levels update! Id={_levelId}");
        
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical();
        
        if (GUILayout.Button("DELETE"))
            if (TryDeleteLevel(_levelId))
                _log?.Invoke($"Levels delete! Id={_levelId}");

        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical();

        if (GUILayout.Button("VERIFY"))
            _log?.Invoke(VerifyLevel());

        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical();

        if (GUILayout.Button("Count All"))
        {
            var count = repository.ReadAll().Count();
            _log?.Invoke($"Potential last level ID: {count - 1};");
        }
        
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }

    private void RebuildGameGrid(bool saveActiveLevelParts)
    {
        var activeSize = new Vector2Int(_levels.GetLength(0), _levels.GetLength(1));
        RebuildGameGrid(activeSize, saveActiveLevelParts);
    }
    
    private void RebuildGameGrid(Vector2Int newSize, bool saveActiveLevelParts)
    {
        var activeLevels = _levels;
        _levels = new TLevel[newSize.x, newSize.y];

        _editorGridSizeX = _levels.GetLength(0);
        _editorGridSizeY = _levels.GetLength(1);
        
        if (saveActiveLevelParts == false)
            return;

        for (var x = 0; x < _levels.GetLength(0) && x < activeLevels.GetLength(0); x++)
        {
            for (var y = 0; y < _levels.GetLength(1) && y < activeLevels.GetLength(1); y++)
            {
                _levels[x, y] = activeLevels[x, y];
            }
        }
    }

    private void ResetUserPanel()
    {
        ResetCustomUserPanel();
        _levelId = 0;
    }
    
    protected virtual void ResetCustomUserPanel() { }
    
    #region Levels CRUD

    private bool TrySaveLevel(int levelId)
    {
        var availableFile = repository.Read(levelId);
        if (availableFile != null)
        {
            _log?.Invoke($"File with ID={levelId} already exist. Save skipped");

            return false;
        }

        var level = GetLevelDataInstance(levelId, _levels);
        repository.CreateById(level);
        _updateAssets?.Invoke();
        
        return true;
    }
    
    protected bool TryLoadLevel(int levelId)
    {
        var level = repository.Read(levelId);
        if (level == null)
        {
            _log?.Invoke($"Can't find level by ID={_levelId}. Load skipped");

            return false;
        }

        OnLoadNewData(level);
        GetLevelsByInstance(level, out var levelsById);
        _levels = levelsById;
        _levelId = levelId;
        RebuildGameGrid(true);

        return true;
    }
    
    private bool TryDeleteLevel(int levelId)
    {
        var level = repository.Read(levelId);
        if (level == null)
        {
            _log?.Invoke($"Can't find level by ID={_levelId}. Delete skipped");

            return false;
        }

        repository.Delete(level);
        _updateAssets?.Invoke();

        return true;
    }

    private bool TryUpdateLevel(int levelId)
    {
        var availableFile = repository.Read(levelId);
        if (availableFile == null)
        {
            _log?.Invoke($"Can't find level by Id={_levelId}. Update skipped");

            return false;
        }

        var level = GetLevelDataInstance(levelId, _levels);
        repository.Update(level);
        _updateAssets?.Invoke();
        
        return true;
    }

    #endregion

    protected abstract TData GetLevelDataInstance(int id, TLevel[,] levels);
    
    protected abstract void GetLevelsByInstance(TData levelData, out TLevel[,] levels);
    
    protected virtual void OnLoadNewData(TData levelData) { }
    
    protected virtual string VerifyLevel()
    {
        return "The current settings do not check the level";
    }

    protected void Print(string message) => _log?.Invoke(message);
}
}