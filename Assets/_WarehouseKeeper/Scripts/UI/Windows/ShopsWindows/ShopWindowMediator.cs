using System;
using System.Threading;
using Game;
using Game.GUI.Windows;
using Game.Localizations;
using Game.Shops;
using WarehouseKeeper.Directors.Game.Analytics.Signals;
using WarehouseKeeper.Directors.Game.UserResources;
using WarehouseKeeper.Directors.UI.Shops;
using WarehouseKeeper.Directors.UI.Windows;
using WarehouseKeeper.UI.Windows.MainWindows;
using Zenject;

namespace WarehouseKeeper.UI.Windows.ShopWindows
{
internal class ShopWindowMediator : BaseMediator<ShopWindowView>
{
    private readonly WindowsDirector _windowsDirector;
    private readonly ShopItemFactory _shopItemFactory;
    private readonly IShopManager _shopManager;
    private readonly ILocalizationManager _localizationManager;
    private readonly ShopDirector _shopDirector;
    private readonly PlayerResourcesDirector _playerResourcesDirector;
    private readonly SignalBus _signalBus;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private ShopItem[] _currencyItems;
    private ShopItem[] _localItems;
    
    public ShopWindowMediator(ShopWindowView window, 
                              WindowsDirector windowsDirector, ShopItemFactory shopItemFactory, IShopManager shopManager, PlayerResourcesDirector playerResourcesDirector, ShopDirector shopDirector, ILocalizationManager localizationManager, SignalBus signalBus) : base(window)
    {
        _windowsDirector = windowsDirector;
        _shopItemFactory = shopItemFactory;
        _shopManager = shopManager;
        _playerResourcesDirector = playerResourcesDirector;
        _shopDirector = shopDirector;
        _localizationManager = localizationManager;
        _signalBus = signalBus;
    }

    public override void OnInitialize()
    {
        window.OnWindowAction += ProceedButtonAction;
        window.PlayerResourcesView.AutoInitialize(_playerResourcesDirector, _localizationManager);
        InitializeLocalProducts();
        InitializeCurrencyProducts();
    }

    public override void OnDestroy()
    {
        window.OnWindowAction -= ProceedButtonAction;
        window.PlayerResourcesView.AutoDispose();
        DisposeAllItems();
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
    }

    private void InitializeLocalProducts()
    {
        _currencyItems = _shopItemFactory.GetLocalItems(window.ProductsRoot);
        foreach (var shopItem in _currencyItems)
        {
            shopItem.OnClickItem += ProcessPurchasingLocalProduct;
        }
    }
    
    private void InitializeCurrencyProducts()
    {
        _localItems = _shopItemFactory.GetCurrencyItems(window.ProductsRoot);
        foreach (var shopItem in _localItems)
        {
            shopItem.OnClickItem += ProcessPurchasingCurrencyProduct;
        }
    }

    private void DisposeAllItems()
    {
        if (_localItems != null)
            foreach (var shopItem in _localItems)
                shopItem.Release();
        if (_currencyItems != null)
            foreach (var shopItem in _currencyItems)
                shopItem.Release();
    }
    private async void ProcessPurchasingCurrencyProduct(ShopItem shopItem)
    {
        window.DisableInteraction();
        var result = await _shopManager.PurchaseProduct(shopItem.product.ProductId);
        window.EnableInteraction();
        if (_cancellationTokenSource.IsCancellationRequested || shopItem.product == null)
            return;
        var productId = shopItem.product.ProductId;
        if (result == PurchaseResult.Success)
            ProcessPayoutReward(shopItem.product, shopItem.product.Rewards);
        else
            window.Informer.Show(_localizationManager.Localize("shop_purchasingError"));
        
        _signalBus.Fire(new PurchaseAmber {productId = productId, result = result});
    }

    private void ProcessPurchasingLocalProduct(ShopItem shopItem)
    {
        var result = _shopDirector.CanBuyProduct(shopItem.product.ProductId);
        if (result)
            ProcessPayoutReward(shopItem.product, shopItem.product.Rewards);
        else
            window.Informer.Show(_localizationManager.Localize("shop_localBoughtError"));
    }

    #region Payouts

    private void ProcessPayoutReward(GameProduct product, ShopProductReward[] rewards)
    {
        foreach (var reward in rewards)
        {
            _signalBus.Fire(new PurchaseProduct
            {
                productId = product.ProductId, reward = reward,
                place = _windowsDirector.GetFirstOrDefaultWindow<MainWindowMediator>() != null ? "Menu" : "Game",
                amberInitValue = _playerResourcesDirector.UserData.Amber.Value,
                hintInitValue = _playerResourcesDirector.UserData.Hints.Value
            });

            switch (reward.type)
            {
                case RewardType.RemoveAds:
                    PayoutRemoveAds();
                    break;
                case RewardType.Hint:
                    PayoutHint(reward.quantity, (int) product.Price);
                    break;
                case RewardType.Amber:
                    PayoutAmber(reward.quantity);
                    break;
                default:
                    Log.Error($"Unknown reward type {reward.type}");
                    break;
            }
        }
    }

    private void PayoutRemoveAds()
    {
        _playerResourcesDirector.UpdateData(data =>
        {
            data.AdsDisable = true;
            Log.Info("Remove ad");
        });
        
        DisposeAllItems();
        InitializeLocalProducts();
        InitializeCurrencyProducts();
    }
    
    private void PayoutHint(int count, int spendAmber)
    {
        _playerResourcesDirector.UpdateData(data =>
        {
            data.Amber.Spend(spendAmber);
            data.Hints.Add(count);
            Log.Info($"Added hints: {count}; Spend amber: {spendAmber}");
        });
    }
    
    private void PayoutAmber(int count)
    {
        _playerResourcesDirector.UpdateData(data =>
        {
            data.Amber.Add(count);
            Log.Info($"Added amber: {count}");
        });
    }
    
    #endregion
    
    #region Button event handler

    private void ProceedButtonAction(ShopWindowAction action)
    {
        switch (action)
        {
            case ShopWindowAction.OnClickCloseYourself:
                _windowsDirector.CloseWindow(this);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(action), action, null);
        }
    }

    #endregion
}
}