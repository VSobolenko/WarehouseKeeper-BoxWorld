using System;
using System.Collections.Generic;
using System.Linq;
using Game.Extensions;
using Game.Repositories;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using WarehouseKeeper.Levels;

namespace WarehouseKeeper.EditorTools.Levels
{
internal class LevelSettingsGenerator : LevelsGenerator<LevelSettings, LevelPiece>
{
    private List<Vector2Int> _walkthrough = new();
    private int _spentHints;
    private int _walkthroughSize = 200;
    private ReorderableList _reorderable;
    private bool _foldoutCustomUserPanel = true;
    private Vector2 _walkthroughScrollPos;
    private int _idMirror;

    public LevelSettingsGenerator(IRepository<LevelSettings> repository, Action<string> log, Vector2Int defaultGridSize,
                                  Action onUpdateAssets) : base(repository, log, defaultGridSize, onUpdateAssets)
    {
        ForceUpdateReorderableList();

    }

    protected override void DrawCustomUserPanel()
    {
        EditorGUILayout.BeginVertical();

        //Create mirror
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("ID Mirror X = ", GUILayout.MaxWidth(75));
        _idMirror = EditorGUILayout.IntField(_idMirror, GUILayout.MaxWidth(100));
        if (GUILayout.Button("Create mirror X"))
            CreateMirrorXLevel(_idMirror);
        if (GUILayout.Button("Create mirror Y"))
            CreateMirrorYLevel(_idMirror);
        if (GUILayout.Button("Create mirror XY"))
            CreateMirrorXYLevel(_idMirror);
        if (GUILayout.Button("Fast random X/Y/XY"))
            CreateAllMirror();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(3, false);
        
        _foldoutCustomUserPanel = EditorGUILayout.Foldout(_foldoutCustomUserPanel, "Hints");

        if (!_foldoutCustomUserPanel)
        {
            EditorGUILayout.EndVertical();
            return;
        }

        //Hint
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Spend Hints = ", GUILayout.MaxWidth(80));
        _spentHints = EditorGUILayout.IntField(_spentHints, GUILayout.MaxWidth(150));
        EditorGUILayout.Space(5);

        EditorGUILayout.LabelField("Hints size = ", GUILayout.MaxWidth(75));
        var newMaxSize = EditorGUILayout.IntField(_walkthroughSize, GUILayout.MaxWidth(100));
        _walkthroughSize = Mathf.Max(newMaxSize, 50);
        EditorGUILayout.Space(5, false);

        EditorGUILayout.LabelField("Walkthrough from top to bottom ", GUILayout.MaxWidth(180));

        if (GUILayout.Button("Hint length"))
            Print($"Hint length = {_walkthrough.Count}");
        if (GUILayout.Button("Receive hint tunel"))
            TryReceiveHintTunel();
        EditorGUILayout.EndHorizontal();

        var maxWidth = GUILayout.MaxHeight(_walkthroughSize);
        var minWidth = GUILayout.MinHeight(_walkthroughSize);

        _walkthroughScrollPos = EditorGUILayout.BeginScrollView(_walkthroughScrollPos, false, false,
                                                                 GUI.skin.horizontalScrollbar,
                                                                 GUI.skin.verticalScrollbar, GUI.skin.box,
                                                                 new[] {maxWidth, minWidth});
        _reorderable.DoLayoutList();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void TryReceiveHintTunel()
    {
        var hintTunel = GameObject.FindObjectOfType<WarehouseKeeper.EditorTools.Levels.HintTunel>();
        if (hintTunel == null)
        {
            Print("Cannot find hint tunel");
        }        
        else
        {
            _walkthrough.Clear();
            _walkthrough.AddRange(hintTunel.GetDirections());
            Print($"Receive complete; Hints length = {_walkthrough.Count}");
        }
    }
    
    protected override void ResetCustomUserPanel()
    {
        _walkthrough.Clear();
        _spentHints = 0;
        _walkthroughSize = 200;
        _idMirror = 0;
    }

    protected override void DrawLevelPart(ref LevelPiece partData, out int countVerticalElements)
    {
        var maxWidth = GUILayout.MaxWidth(90);
        var entityStyle = new GUIStyle(EditorStyles.popup)
        {
            normal = {textColor = GetColor(partData.Type)}, active = {textColor = GetColor(partData.Type)},
            focused = {textColor = GetColor(partData.Type)}, hover = {textColor = GetColor(partData.Type)}
        };
        var pieceStyle = new GUIStyle(EditorStyles.popup)
        {
            normal = {textColor = GetColor(partData.Start)}, active = {textColor = GetColor(partData.Start)},
            focused = {textColor = GetColor(partData.Start)}, hover = {textColor = GetColor(partData.Start)}
        };
        
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("World", maxWidth);
        partData.Type = (PieceType) EditorGUILayout.EnumPopup(partData.Type, entityStyle, maxWidth);
        partData.Start = (GameEntityType) EditorGUILayout.EnumPopup(partData.Start, pieceStyle, maxWidth);
        EditorGUILayout.EndVertical();

        countVerticalElements = 2;
    }

    private Color GetColor(PieceType type)
    {
        switch (type)
        {
            case PieceType.PlayZone:
                return Color.white;
            case PieceType.Wall:
                return Color.black;
            case PieceType.Empty:
                return Color.gray;
            case PieceType.Target:
                return Color.yellow;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }    
    }

    private Color GetColor(GameEntityType type)
    {
        switch (type)
        {
            case GameEntityType.None:
                return Color.gray;
            case GameEntityType.Player:
                return Color.red;
            case GameEntityType.Box:
                return Color.blue;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
    
    protected override LevelSettings GetLevelDataInstance(int id, LevelPiece[,] levels)
    {
        var serializableVectors = new Vector2IntSerializable[_walkthrough.Count];
        for (var i = 0; i < _walkthrough.Count; i++)
            serializableVectors[i] = Vector2IntSerializable.Convert(_walkthrough[i]);

        return new LevelSettings
        {
            Id = id,
            Pieces = levels,
            Walkthrough = serializableVectors,
            SpentHints = _spentHints,
        };
    }

    protected override void GetLevelsByInstance(LevelSettings levelData, out LevelPiece[,] levels)
    {
        levels = levelData.Pieces;
    }

    protected override void OnLoadNewData(LevelSettings levelData)
    {
        if (levelData.Walkthrough == null)
        {
            _walkthrough.Clear();
            return;
        }

        _spentHints = levelData.SpentHints;
        
        _walkthrough = new List<Vector2Int>(levelData.Walkthrough.Length);
        for (int i = 0; i < levelData.Walkthrough.Length; i++)
            _walkthrough.Add(levelData.Walkthrough[i].GetVector2Int());
        
        ForceUpdateReorderableList();
    }

    private void ForceUpdateReorderableList()
    {
        _reorderable = new ReorderableList(_walkthrough, typeof(Vector2Int))
        {
            drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Solution"),
            drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var rectX = new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight);
                var rectY = new Rect(rect.x + 100, rect.y, 100, EditorGUIUtility.singleLineHeight);

                var x = EditorGUI.IntField(rectX, _walkthrough[index].x);
                var y = EditorGUI.IntField(rectY, _walkthrough[index].y);
                if (x != 0 && y != 0)
                {
                    x = 0;
                    y = 0;
                    Print("Unsupported direction");
                }
                _walkthrough[index] = new Vector2Int(Mathf.Clamp(x, -1, 1), Mathf.Clamp(y, -1, 1));
            },
        };
    }

    private void CreateAllMirror()
    {
        var allId = repository.ReadAll().Select(x => x.Id).ToList();
        var random = new System.Random();
        allId.Shuffle(random);
        foreach (var id in allId)
            CreateMirrorXLevel(id);
        
        allId.Shuffle(random);
        foreach (var id in allId)
            CreateMirrorYLevel(id);

        allId.Shuffle(random);
        foreach (var id in allId)
            CreateMirrorXYLevel(id);
    }
    
    private void CreateMirrorXLevel(int existLevelId)
    {
        var settings = repository.Read(existLevelId);

        if (settings == null)
        {
            Print($"Can't find settings ID={existLevelId} to mirror");
            return;
        }

        var mirroredPieces = new LevelPiece[settings.Pieces.GetLength(0), settings.Pieces.GetLength(1)];
        {
            var lengthX = mirroredPieces.GetLength(0);
            var lengthY = mirroredPieces.GetLength(1);
            for (var x = 0; x < lengthX; x++)
            {
                for (var y = 0; y < lengthY; y++)
                {
                    mirroredPieces[x, y] = settings.Pieces[lengthX - x - 1, y];
                }
            }
        }
        
        var mirroredWalkthrough = new Vector2IntSerializable[settings.Walkthrough.Length];
        {
            var length = settings.Walkthrough.Length;
            for (var x = 0; x < length; x++)
            {
                mirroredWalkthrough[x] =
                    new Vector2IntSerializable(settings.Walkthrough[x].x * -1, settings.Walkthrough[x].y);
            }
        }
        var mirroredSettings = new LevelSettings
        {
            Pieces = mirroredPieces,
            Walkthrough = mirroredWalkthrough,
            SpentHints = settings.SpentHints,
        };

        mirroredSettings.Id = repository.Create(mirroredSettings);
        if (TryLoadLevel(mirroredSettings.Id))
            Print($"Level mirrored X! Source level ID={existLevelId}. New saved level ID={mirroredSettings.Id}");
    }

    private void CreateMirrorYLevel(int existLevelId)
    {
        var settings = repository.Read(existLevelId);

        if (settings == null)
        {
            Print($"Can't find settings ID={existLevelId} to mirror");
            return;
        }

        var mirroredPieces = new LevelPiece[settings.Pieces.GetLength(0), settings.Pieces.GetLength(1)];
        {
            var lengthX = mirroredPieces.GetLength(0);
            var lengthY = mirroredPieces.GetLength(1);
            for (var x = 0; x < lengthX; x++)
            {
                for (var y = 0; y < lengthY; y++)
                {
                    mirroredPieces[x, y] = settings.Pieces[x, lengthY - y - 1];
                }
            }
        }
        
        var mirroredWalkthrough = new Vector2IntSerializable[settings.Walkthrough.Length];
        {
            var length = settings.Walkthrough.Length;
            for (var x = 0; x < length; x++)
            {
                mirroredWalkthrough[x] =
                    new Vector2IntSerializable(settings.Walkthrough[x].x, settings.Walkthrough[x].y * -1);
            }
        }
        var mirroredSettings = new LevelSettings
        {
            Pieces = mirroredPieces,
            Walkthrough = mirroredWalkthrough,
            SpentHints = settings.SpentHints,
        };

        mirroredSettings.Id = repository.Create(mirroredSettings);
        if (TryLoadLevel(mirroredSettings.Id))
            Print($"Level mirrored Y! Source level ID={existLevelId}. New saved level ID={mirroredSettings.Id}");
    }

    private void CreateMirrorXYLevel(int existLevelId)
    {
        var settings = repository.Read(existLevelId);

        if (settings == null)
        {
            Print($"Can't find settings ID={existLevelId} to mirror");
            return;
        }

        var mirroredPieces = new LevelPiece[settings.Pieces.GetLength(0), settings.Pieces.GetLength(1)];
        {
            var lengthX = mirroredPieces.GetLength(0);
            var lengthY = mirroredPieces.GetLength(1);
            for (var x = 0; x < lengthX; x++)
            {
                for (var y = 0; y < lengthY; y++)
                {
                    mirroredPieces[x, y] = settings.Pieces[lengthX - x - 1, lengthY - y - 1];
                }
            }
        }
        
        var mirroredWalkthrough = new Vector2IntSerializable[settings.Walkthrough.Length];
        {
            var length = settings.Walkthrough.Length;
            for (var x = 0; x < length; x++)
            {
                mirroredWalkthrough[x] =
                    new Vector2IntSerializable(settings.Walkthrough[x].x * -1, settings.Walkthrough[x].y * -1);
            }
        }
        var mirroredSettings = new LevelSettings
        {
            Pieces = mirroredPieces,
            Walkthrough = mirroredWalkthrough,
            SpentHints = settings.SpentHints,
        };

        mirroredSettings.Id = repository.Create(mirroredSettings);
        if (TryLoadLevel(mirroredSettings.Id))
            Print($"Level mirrored XY! Source level ID={existLevelId}. New saved level ID={mirroredSettings.Id}");
    }
}
}