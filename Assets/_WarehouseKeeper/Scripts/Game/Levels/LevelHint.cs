using Game;
using Game.AssetContent;
using Game.Pools;
using UnityEngine;
using WarehouseKeeper.Directors.Game.Hints;
using WarehouseKeeper.UI.Windows;

namespace WarehouseKeeper.Levels
{
internal class LevelHint : PrefabProviderByAddress<DirectionalArrow>
{
    private const string DirectionArrowAddressableKey = "DirectionalArrow";
    
    private readonly IObjectPoolManager _objectPool;
    private readonly LevelSettings _levelSettings;
    private readonly LevelDirector _levelDirector;

    public bool HintIsActive => _activeMoveState <= _maxStage;
    public Vector2 TargetDirection => _levelSettings.Walkthrough[_activeMoveState].GetVector2();
    public bool IsComplete => _activeMoveState >= _maxStage && _lastStateViewed;

    private DirectionalArrow _cachedArrow;
    private bool _lastStateViewed;
    private int _activeMoveState;
    private readonly int _maxStage;

    public LevelHint(IAddressablesManager addressablesManager, 
                     IObjectPoolManager objectPool,
                     LevelRepositoryDirector levelRepositoryDirector, 
                     LevelDirector levelDirector) 
        : base(addressablesManager)
    {
        _objectPool = objectPool;
        _levelSettings = levelRepositoryDirector.GetLevelSetting(levelDirector.ActiveLevel.LevelId);
        _levelDirector = levelDirector;

        _activeMoveState = 0;
        _levelDirector.ActiveLevel.Data.CountActiveHints++;
        levelRepositoryDirector.SaveLevelData(_levelDirector.ActiveLevel.Data);
        _maxStage = GetActiveMaxStage();
    }

    public void SetCustomStage(int stage)
    {
        _activeMoveState = stage;
    }
    
    private int GetActiveMaxStage()
    {
        if (_levelSettings.SpentHints == 0)
        {
            Log.Error("Hint not setup");

            return _levelSettings.Walkthrough.Length;
        }
        var maximumHints = _levelSettings.Walkthrough.Length;
        var delimiter = _levelSettings.SpentHints;
        var countActivated = _levelDirector.ActiveLevel.Data.CountActiveHints;
        var stage = maximumHints / delimiter * countActivated;
        
        var activeStage = Mathf.Clamp(stage, 0, maximumHints);

        return activeStage - 1;
    }
    
    public void IncreaseState()
    {
        _activeMoveState++;
    }
    
    private void SetHintState(int moveState)
    {
        if (moveState >= _levelSettings.Walkthrough.Length)
            Log.Error("State overflow");
        
        var state = Mathf.Min(moveState, _levelSettings.Walkthrough.Length);
        if (state == _maxStage)
            _lastStateViewed = true;

        Log.Write($"Activate state:{state}; MaxStage={_maxStage}; ActiveHintStage={_levelDirector.ActiveLevel.Data.CountActiveHints}");
        var activeDirection = _levelSettings.Walkthrough[state].GetVector2();
        var arrow = _cachedArrow == null ? GetArrow() : _cachedArrow;
        var playerEntity = _levelDirector.ActiveLevel.GetPlayer();
        arrow.SetStartPosition(playerEntity.Entity.transform.position + new Vector3(1f, 1f, 1f) / 2f);
        arrow.Enable(activeDirection);
    }

    private void SetArrowView()
    {
        var direction = _levelDirector.ActiveLevel.Hint.GetActiveDirection();
        var arrow = _cachedArrow == null ? GetArrow() : _cachedArrow;
        var playerEntity = _levelDirector.ActiveLevel.GetPlayer();
        arrow.SetStartPosition(playerEntity.Entity.transform.position + new Vector3(1f, 1f, 1f) / 2f);
        arrow.Enable(direction);
    }
    
    public void EnableView()
    {
        SetArrowView();
    }

    public void DisableView()
    {
        if (_cachedArrow != null)
        {
            _cachedArrow.Release();
            _cachedArrow = null;
        }
    }
    
    private DirectionalArrow GetArrow()
    {
        var prefab = GetPrefab(DirectionArrowAddressableKey);
        var arrow = _objectPool.Get(prefab);
        _cachedArrow = arrow;
        return arrow;
    }

    public void DisposeHint()
    {
        if (_cachedArrow != null)
        {
            _cachedArrow.Release();
            _cachedArrow = null;
        }
    }
}
}