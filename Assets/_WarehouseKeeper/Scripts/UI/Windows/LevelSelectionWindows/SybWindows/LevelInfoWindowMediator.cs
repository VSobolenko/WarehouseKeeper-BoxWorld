using System;
using System.Text;
using Game;
using Game.GUI.Windows;
using Game.Localizations;
using WarehouseKeeper._WarehouseKeeper.Scripts.UI.Windows.LevelSelectionWindows.SybWindows.Components;
using WarehouseKeeper.Directors.UI.Windows;
using WarehouseKeeper.Levels;

namespace WarehouseKeeper._WarehouseKeeper.Scripts.UI.Windows.LevelSelectionWindows.SybWindows
{
internal class LevelInfoWindowMediator : BaseMediator<LevelInfoWindowView>
{
    private readonly WindowsDirector _windowsDirector;
    private readonly LevelRepositoryDirector _levelRepositoryDirector;
    private readonly ILocalizationManager _localizationManager;

    public LevelInfoWindowMediator(LevelInfoWindowView window, WindowsDirector windowsDirector, LevelRepositoryDirector levelRepositoryDirector, ILocalizationManager localizationManager) : base(window)
    {
        _windowsDirector = windowsDirector;
        _levelRepositoryDirector = levelRepositoryDirector;
        _localizationManager = localizationManager;
    }

    public override void OnInitialize()
    {
        window.OnWindowAction += ProceedButtonAction;
    }

    public override void OnDestroy()
    {
        window.OnWindowAction -= ProceedButtonAction;
    }

    public void Setup(int levelID)
    {
        var data = _levelRepositoryDirector.GetLevelData(levelID);
        if (data == null)
        {
            Log.InternalError();
            _windowsDirector.CloseWindow(this);
            return;
        }

        window.LevelNumber.text = _localizationManager.LocalizeFormat("levelInfo_number", data.Id + 1);
        window.CountMoves.text = _localizationManager.LocalizeFormat("levelInfo_moves", data.CountMoves);
        window.BestMoves.text = _localizationManager.LocalizeFormat("levelInfo_bestMoves", data.BestMoves);
        window.CountPushes.text = _localizationManager.LocalizeFormat("levelInfo_pushes", data.CountPushes);
        window.BestPushes.text = _localizationManager.LocalizeFormat("levelInfo_bestPushes", data.BestPushes);
        window.TimeSpent.text = _localizationManager.LocalizeFormat("levelInfo_timeSpent", GetViewTime(data.TimeSpent));
        window.BestTime.text = _localizationManager.LocalizeFormat("levelInfo_bestTime", GetViewTime(data.BestTime));
        
        window.BestMoves.gameObject.SetActive(data.BestMoves != int.MaxValue);
        window.BestPushes.gameObject.SetActive(data.BestPushes != int.MaxValue);
        window.BestTime.gameObject.SetActive(data.BestTime != TimeSpan.MaxValue);
        window.Stars.StaticSetup(data.StarsReceived);
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
    
    #region Button event handler

    private void ProceedButtonAction(LevelInfoWindowAction action)
    {
        switch (action)
        {
            case LevelInfoWindowAction.OnClickCancel:
                _windowsDirector.CloseWindow(this);
                break;
            default:
                Log.Error($"Unknown action {action}");
                break;        
        }
    }

    #endregion
}
}