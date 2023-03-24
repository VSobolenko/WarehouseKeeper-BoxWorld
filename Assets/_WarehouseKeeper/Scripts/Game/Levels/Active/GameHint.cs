using System;
using UnityEngine;
using WarehouseKeeper.Extension;

namespace WarehouseKeeper.Levels
{
internal class GameHint
{
    private readonly Vector2[] _walkthrough;
    private readonly int _spendHints;
    private int _maxHintStage;
    private int _activeHintStage;

    public bool IsActive { get; private set; }
    public bool IsComplete => IsActive && _activeHintStage > _maxHintStage;
    public bool LastStateWithHint => IsActive && IsComplete && _activeHintStage - 1 == _maxHintStage;
    public bool InProgress => IsActive && IsComplete == false;

    public GameHint(LevelSettings levelSettings)
    {
        IsActive = false;
        _spendHints = levelSettings.SpentHints;
        _walkthrough = new Vector2[levelSettings.Walkthrough.Length];
        for (var i = 0; i < _walkthrough.Length; i++)
            _walkthrough[i] = levelSettings.Walkthrough[i].GetVector2();
    }

    public void IncreaseStep()
    {
        _activeHintStage++;
    }
    
    public void DecreaseStep()
    {
        _activeHintStage--;
    }
    
    public void Activate(int countActiveHints, int activeStage)
    {
        IsActive = true;
        Setup(countActiveHints, activeStage);
    }

    private void Setup(int countActiveHints, int activeHintStage)
    {
        if (_spendHints == 0)
        {
            Log.WriteError("Hint not setup");
            _activeHintStage = 0;
            _maxHintStage = 0;
            return;
        }
        var maximumHints = _walkthrough.Length;
        var delimiter = _spendHints;
        var countActivated = countActiveHints;
        var stage = maximumHints / delimiter * countActivated;
        
        var activeStage = Mathf.Clamp(stage, 0, maximumHints);

        _activeHintStage = activeHintStage;
        _maxHintStage = activeStage - 1;
    }

    public Vector2 GetActiveDirection()
    {
        if (_activeHintStage >= _walkthrough.Length)
            Log.WriteWarning("Index out of hit stage");

        var stage = Mathf.Clamp(_activeHintStage, 0, _walkthrough.Length - 1);
        if (stage >= _walkthrough.Length || stage < 0)
        {
            Log.WriteError("Stage error! Direction reset");
            return Vector2.zero;
        }
        return _walkthrough[stage];
    }
}
}