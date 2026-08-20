using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Game.AssetContent;
using Game.Localizations;
using Game.Pools;
using Game.Shops;
using Game;
using UnityEngine;
using WarehouseKeeper.Directors.Game.Analytics.Signals;
using WarehouseKeeper.Directors.Game.UserResources;
using WarehouseKeeper.Directors.UI.Shops;
using WarehouseKeeper._WarehouseKeeper.Scripts.Shops.Monetization.Purchasing;
using Zenject;

namespace WarehouseKeeper.UI.Windows.ShopWindows
{
internal class ShopItemFactory : IInitializable, IDisposable
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly IResourceManager _resourceManagement;
    private readonly IShopCatalog _shopCatalog;
    private readonly IPurchasingDirector _purchasingDirector;
    private readonly IObjectPoolManager _objectPool;
    private readonly ILocalizationManager _localizationManager;
    private readonly ShopDirector _shopDirector;
    private readonly PlayerResourcesDirector _playerResourcesDirector;
    private readonly SignalBus _signalBus;

    public ShopItemFactory(IResourceManager resourceManagement, IObjectPoolManager objectPool,
                           IShopCatalog shopCatalog, IPurchasingDirector purchasingDirector,
                           ILocalizationManager localizationManager,
                           ShopDirector shopDirector, PlayerResourcesDirector playerResourcesDirector,
                           SignalBus signalBus)
    {
        _resourceManagement = resourceManagement;
        _objectPool = objectPool;
        _shopCatalog = shopCatalog;
        _purchasingDirector = purchasingDirector;
        _localizationManager = localizationManager;
        _shopDirector = shopDirector;
        _playerResourcesDirector = playerResourcesDirector;
        _signalBus = signalBus;
    }

    public async void Initialize()
    {
        try
        {
            await _purchasingDirector.InitializeAsync(_cancellationTokenSource.Token);
        }
        catch (Exception exception)
        {
            Log.Error(exception.Message);
        }

        await PrepareItemsAsync();
        _signalBus.Fire(new ShopEvent
        {
            message = $"Initialize complete: " +
                      $"Count products={_shopCatalog.Products.Count}; " +
                      $"Id={string.Join(";", _shopCatalog.Products.Select(x => x.ProductId))}",
            time = Time.time.ToString(CultureInfo.InvariantCulture),
        });
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
    }

    public async Task PrepareItemsAsync()
    {
        var uniqueCurrencyAddressableKeys = _shopCatalog.Products.Select(x => x.AddressableItemKey).Distinct();
        var uniqueLocalAddressableKeys = _shopDirector.LocalProducts.Select(x => x.AddressableItemKey).Distinct();

        var keys = uniqueCurrencyAddressableKeys.Union(uniqueLocalAddressableKeys);

        foreach (var addressableKey in keys)
        {
            if (string.IsNullOrEmpty(addressableKey))
                Log.Error("Null addressable item key");

            if (_cancellationTokenSource.IsCancellationRequested)
                return;
            var item = await GetPrefabAsync<ShopItem>(addressableKey, _cancellationTokenSource.Token);

            if (_cancellationTokenSource.IsCancellationRequested)
                return;

            if (item == null)
            {
                Log.InternalError();

                continue;
            }

            await _objectPool.PrepareAsync(item, 1, token: _cancellationTokenSource.Token);
        }
    }

    public ShopItem[] GetCurrencyItems(Transform root)
    {
        var products = _shopCatalog.Products.ToArray();
        var items = new List<ShopItem>(products.Length);

        for (var i = 0; i < products.Length; i++)
        {
            var adsDisable = _playerResourcesDirector.UserData.AdsDisable;
            var product = products[i];

            if (product.Rewards.Count(x => x.type == RewardType.RemoveAds) > 0 && adsDisable)
                continue;

            var shopItem = GetCurrencyItem(product, root);
            items.Add(shopItem);
        }

        return items.ToArray();
    }

    public ShopItem GetCurrencyItem(GameProduct product, Transform root)
    {
        var prefab = GetPrefab<ShopItem>(product.AddressableItemKey);
        var instance = _objectPool.Get(prefab, root);
        instance.transform.localScale = prefab.transform.localScale;
        var displayedName = _localizationManager.Localize(product.LocalizationKeyName);
        instance.Setup(product, product.Icon, $"{product.Price}$", displayedName);

        return instance;
    }

    public ShopItem[] GetLocalItems(Transform root)
    {
        var items = new ShopItem[_shopDirector.LocalProducts.Length];

        for (var i = 0; i < _shopDirector.LocalProducts.Length; i++)
            items[i] = GetLocalItem(_shopDirector.LocalProducts[i], root);

        return items;
    }

    public ShopItem GetLocalItem(LocalGameProduct product, Transform root)
    {
        var prefab = GetPrefab<ShopItem>(product.AddressableItemKey);
        var instance = _objectPool.Get(prefab, root);
        instance.transform.localScale = prefab.transform.localScale;
        var displayedName = _localizationManager.Localize(product.LocalizationKeyName);
        instance.Setup(product, product.Icon, $"{product.Price}", displayedName);

        return instance;
    }

    private async Task<T> GetPrefabAsync<T>(string addressableKey, CancellationToken token) where T : class
    {
        var prefab = await _resourceManagement.LoadAssetAsync<GameObject>(addressableKey);

        if (token.IsCancellationRequested)
            return null;

        if (prefab == null)
        {
            Log.Error($"Addressable key prefab {addressableKey} missing");

            return null;
        }

        var levelSelectionItem = prefab.GetComponent<T>();

        if (levelSelectionItem == null)
        {
            Log.Error($"Component [LevelSelectionItem] missing from {prefab.name} gameObject");

            return null;
        }

        return levelSelectionItem;
    }

    private T GetPrefab<T>(string addressableKey) where T : class
    {
        var prefab = _resourceManagement.LoadAsset<GameObject>(addressableKey);

        if (prefab == null)
        {
            Log.Error($"Addressable key prefab {addressableKey} missing");

            return null;
        }

        var levelSelectionItem = prefab.GetComponent<T>();

        if (levelSelectionItem == null)
        {
            Log.Error($"Component [LevelSelectionItem] missing from {prefab.name} gameObject");

            return null;
        }

        return levelSelectionItem;
    }
}
}