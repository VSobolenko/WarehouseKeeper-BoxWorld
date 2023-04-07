using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Game;
using Game.AssetContent;
using Game.Localizations;
using Game.Pools;
using Game.Shops;
using UnityEngine;
using WarehouseKeeper.Directors.Game.UserResources;
using WarehouseKeeper.Directors.UI.Shops;
using Zenject;

namespace WarehouseKeeper.UI.Windows.ShopWindows
{
internal class ShopItemFactory : IInitializable, IDisposable
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly IAddressablesManager _addressablesManager;
    private readonly IShopManager _shopManager;
    private readonly IObjectPoolManager _objectPool;
    private readonly ILocalizationManager _localizationManager;
    private readonly ShopDirector _shopDirector;
    private readonly PlayerResourcesDirector _playerResourcesDirector;

    public ShopItemFactory(IAddressablesManager addressablesManager, IObjectPoolManager objectPool,
                           IShopManager shopManager, ILocalizationManager localizationManager,
                           ShopDirector shopDirector, PlayerResourcesDirector playerResourcesDirector)
    {
        _addressablesManager = addressablesManager;
        _objectPool = objectPool;
        _shopManager = shopManager;
        _localizationManager = localizationManager;
        _shopDirector = shopDirector;
        _playerResourcesDirector = playerResourcesDirector;
    }

    public async void Initialize()
    {
        await _shopManager.Initialize();
        await PrepareItemsAsync();
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
    }

    public async Task PrepareItemsAsync()
    {
        var uniqueCurrencyAddressableKeys = _shopManager.Products.Select(x => x.AddressableItemKey).Distinct();
        var uniqueLocalAddressableKeys = _shopDirector.LocalProducts.Select(x => x.AddressableItemKey).Distinct();

        var keys = uniqueCurrencyAddressableKeys.Union(uniqueLocalAddressableKeys);
        
        foreach (var addressableKey in keys)
        {
            if (string.IsNullOrEmpty(addressableKey))
                Log.WriteError("Null addressable item key");

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
            await _objectPool.PrepareAsync(item, 1, _cancellationTokenSource.Token);
        }
    }

    public ShopItem[] GetCurrencyItems(Transform root)
    {
        var items = new List<ShopItem>(_shopManager.Products.Count - 1);

        for (var i = 0; i < _shopManager.Products.Count; i++)
        {
            var adsDisable = _playerResourcesDirector.UserData.AdsDisable;
            if (_shopManager.Products.ElementAt(i).Rewards.Count(x => x.type == RewardType.RemoveAds) > 0 && adsDisable)
                continue;
            items.Add(GetCurrencyItem(_shopManager.Products.ElementAt(i), root));
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
    
    protected async Task<T> GetPrefabAsync<T>(string addressableKey, CancellationToken token) where T : class
    {
        var prefab = await _addressablesManager.LoadAssetAsync<GameObject>(addressableKey);

        if (token.IsCancellationRequested)
            return null;

        if (prefab == null)
        {
            Log.WriteError($"Addressable key prefab {addressableKey} missing");
            return null;
        }

        var levelSelectionItem = prefab.GetComponent<T>();
        
        if (levelSelectionItem == null)
        {
            Log.WriteError($"Component [LevelSelectionItem] missing from {prefab.name} gameObject");
            return null;
        }

        return levelSelectionItem;
    }
    
    protected T GetPrefab<T>(string addressableKey) where T : class
    {
        var prefab = _addressablesManager.LoadAsset<GameObject>(addressableKey);
        
        if (prefab == null)
        {
            Log.WriteError($"Addressable key prefab {addressableKey} missing");
            return null;
        }

        var levelSelectionItem = prefab.GetComponent<T>();
        
        if (levelSelectionItem == null)
        {
            Log.WriteError($"Component [LevelSelectionItem] missing from {prefab.name} gameObject");
            return null;
        }

        return levelSelectionItem;
    }
}
}