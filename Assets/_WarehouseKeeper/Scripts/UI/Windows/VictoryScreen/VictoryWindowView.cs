using System;
using System.Text;
using Game.GUI.Windows;
using Game.Localizations;
using TMPro;
using UnityEngine;
using WarehouseKeeper.Directors.UI.Windows.VictoryScreen.Components;
using WarehouseKeeper.Levels;
using WarehouseKeeper.UI.Windows;

namespace WarehouseKeeper.Directors.UI.Windows.VictoryScreen
{
internal class VictoryWindowView : WindowUI
{
    [SerializeField] private TextMeshProUGUI _levelNumber;
    [SerializeField] private TextMeshProUGUI _countMoves;
    [SerializeField] private TextMeshProUGUI _countBestMoves;
    [SerializeField] private TextMeshProUGUI _elapsedTime;
    [SerializeField] private TextMeshProUGUI _bestTime;
    [SerializeField] private StarsView _stars;
    
    [SerializeField] private GameObject _nextLevelRoot;
    [SerializeField] private VictoryWindowButton[] _windowButtons;

    public event Action<VictoryWindowAction> OnWindowAction;

    private void Start()
    {
        foreach (var button in _windowButtons)
            button.OnClickButton += ClickWindowAction;
    }

    private void OnDestroy()
    {
        foreach (var button in _windowButtons)
            button.OnClickButton -= ClickWindowAction;
    }

    public void Setup(LevelData levelData, LevelStatistics levelStatistics, ILocalizationManager localizationManager)
    {
        _stars.StaticSetup(levelData.StarsReceived);
        _levelNumber.text = localizationManager.LocalizeFormat("victory_level", levelData.Id + 1);
        
        _countMoves.text = localizationManager.LocalizeFormat("victory_moves", levelStatistics.moves);
        _countBestMoves.text = localizationManager.LocalizeFormat("victory_bestMoves", levelData.BestMoves);
        
        _elapsedTime.text = localizationManager.LocalizeFormat("victory_elapsedTime", GetViewTime(levelStatistics.passedTime));
        _bestTime.text = localizationManager.LocalizeFormat("victory_bestTime", GetViewTime(levelData.BestTime));
    }

    private string GetViewTime(TimeSpan time)
    {
        var viewTime = new StringBuilder(8);
        Configure(time.Days);
        Configure(time.Hours);
        Configure(time.Minutes, true);
        Configure(time.Seconds, true);

        void Configure(int value, bool required = false)
        { 
            if (value <= 0 && viewTime.Length <= 0 && required == false) return;
            if (viewTime.Length > 0)
                viewTime.Append(':');
            if (value < 10)
                viewTime.Append('0');
            viewTime.Append(value);
        }

        return viewTime.ToString();
    }
    
    public void SetActiveNextLevel(bool value) => _nextLevelRoot.SetActive(value);
    
    private void ClickWindowAction(VictoryWindowAction action) => OnWindowAction?.Invoke(action);
    
#if UNITY_EDITOR
    
    [ContextMenu("Collect window buttons"),]
    private void CollectWindowButtons()
    {
        _windowButtons = GetComponentsInChildren<VictoryWindowButton>(true);

        UnityEditor.EditorUtility.SetDirty(this);
    }
    
#endif
}
}