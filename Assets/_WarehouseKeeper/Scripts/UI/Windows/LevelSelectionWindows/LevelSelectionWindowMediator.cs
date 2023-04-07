using Game;
using Game.GUI.Windows;
using WarehouseKeeper.Directors.Game;
using WarehouseKeeper.Directors.Game.Analytics.Signals;
using WarehouseKeeper.Directors.Game.UserResources;
using WarehouseKeeper.Directors.UI.Windows;
using WarehouseKeeper.Levels;
using WarehouseKeeper.UI.Windows.LevelSelection;
using Zenject;

namespace WarehouseKeeper.UI.Windows.LevelSelections
{
internal class LevelSelectionWindowMediator : BaseMediator<LevelSelectionWindowView>
{
    private readonly LevelSelectionItemFactory _itemFactory;
    private readonly WindowsDirector _windowsDirector;
    private readonly LevelRepositoryDirector _levelRepositoryDirector;
    private readonly PlayerResourcesDirector _playerResourcesDirector;
    private readonly GameDirector _gameDirector;
    private readonly SignalBus _signalBus;
    private LevelSelectionItem[] _items;

    public LevelSelectionWindowMediator(LevelSelectionWindowView window, 
                                        WindowsDirector windowsDirector,
                                        LevelSelectionItemFactory itemFactory, LevelRepositoryDirector levelRepositoryDirector, PlayerResourcesDirector playerResourcesDirector, GameDirector gameDirector, SignalBus signalBus) : base(window)
    {
        _windowsDirector = windowsDirector;
        _itemFactory = itemFactory;
        _levelRepositoryDirector = levelRepositoryDirector;
        _playerResourcesDirector = playerResourcesDirector;
        _gameDirector = gameDirector;
        _signalBus = signalBus;
    }

    public override void OnInitialize()
    {
        window.OnWindowAction += ProceedButtonAction;
        window.PlayerResourcesView.AutoInitialize(_playerResourcesDirector);
        _items = _itemFactory.GetItems(window.ItemsRoot);
        foreach (var levelSelectionItem in _items)
        {
            levelSelectionItem.OnClickItem += ClickItem;
            levelSelectionItem.OnClickInfo += ClickItemInfo;
        }
    }

    public override void OnDestroy()
    {
        window.OnWindowAction -= ProceedButtonAction;
        window.PlayerResourcesView.AutoDispose();
        foreach (var levelSelectionItem in _items)
        {
            levelSelectionItem.OnClickItem -= ClickItem;
            levelSelectionItem.OnClickInfo -= ClickItemInfo;
            levelSelectionItem.Release();
        }
    }
    
    private void ClickItem(LevelSelectionItem item)
    {
        var levelData = _levelRepositoryDirector.GetLevelData(item.LevelId);
        if (levelData == null)
            TryBuyLevel(item);
        else
            StartLevelById(item.LevelId);
    }

    private void TryBuyLevel(LevelSelectionItem item)
    {
        var prevLevel = _levelRepositoryDirector.GetLevelData(item.LevelId - 1);
        if (prevLevel == null)
            return;
        if (_playerResourcesDirector.UserData.Amber.CanSpend(item.LevelPrice))
        {
            _levelRepositoryDirector.CreateEmptyData(item.LevelId);
            _playerResourcesDirector.UpdateData(data =>
            {
                data.Amber.Spend(item.LevelPrice);

            });
            foreach (var levelSelectionItem in _items)
                _itemFactory.UpdateItemState(levelSelectionItem);
            
            _signalBus.Fire(new UnlockLevelByAmber {levelId = item.LevelId});
        }
        else
        {
            _windowsDirector.OpenShopWindow();
        }
    }

    private void StartLevelById(int id)
    {
        _windowsDirector.CloseWindows();
        _gameDirector.StartLevel(id);
    }
    
    private void ClickItemInfo(LevelSelectionItem item)
    {
        if (item == null)
        {
            Log.InternalError();
            return;
        }

        _windowsDirector.OpenLevelInfoWindow(item.LevelId);
    }
    
    #region Button event handler

    private void ProceedButtonAction(LevelSelectionWindowAction action)
    {
        switch (action)
        {
            case LevelSelectionWindowAction.OnClickCloseYourself:
                _windowsDirector.CloseWindow(this);
                break;
            case LevelSelectionWindowAction.OnClickAmber:
                _windowsDirector.OpenShopWindow();
                break;
            default:
                Log.WriteError($"Unknown action {action}");
                break;        
        }
    }

    #endregion
}
}