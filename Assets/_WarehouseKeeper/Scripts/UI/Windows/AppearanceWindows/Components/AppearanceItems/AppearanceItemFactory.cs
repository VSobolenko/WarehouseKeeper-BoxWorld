using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using WarehouseKeeper.AssetContent;
using WarehouseKeeper.Directors;
using WarehouseKeeper.Directors.Game.UserResources;
using WarehouseKeeper.Extension;
using WarehouseKeeper.Pools;
using WarehouseKeeper.UI.Windows;
using Zenject;

namespace WarehouseKeeper._WarehouseKeeper.Scripts.UI.Windows.AppearanceWindows.Components.AppearanceItems
{
internal class AppearanceItemFactory : PrefabProviderByAddress<AppearanceItem>, IInitializable, IDisposable
{
    private readonly IObjectPoolManager _objectPool;
    private readonly PlayerResourcesDirector _playerResources;
    private readonly ResourcesDirector _resourcesDirector;

    private const string AddressableItemKey = "AppearanceItem";
    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    public AppearanceItemFactory(IAddressablesManager addressablesManager, 
                                 IObjectPoolManager objectPool, 
                                 PlayerResourcesDirector playerResources,
                                 ResourcesDirector resourcesDirector) : base(addressablesManager)
    {
        _objectPool = objectPool;
        _playerResources = playerResources;
        _resourcesDirector = resourcesDirector;
    }

    public async void Initialize()
    {
        await PrepareItemsAsync();
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
    }
    
    public async Task PrepareItemsAsync()
    {
        var maxCountElements = Mathf.Max(_resourcesDirector.AnimationsAppearance.Items?.Count(x => x.accessible) ?? 0,
                                         _resourcesDirector.BoxAppearance.Items?.Count(x => x.accessible) ?? 0,
                                         _resourcesDirector.EffectsAppearance.Items?.Count(x => x.accessible) ?? 0,
                                         _resourcesDirector.UserSkinsAppearance.Items?.Count(x => x.accessible) ?? 0);
        var item = await GetPrefabAsync(AddressableItemKey);
        if (item == null)
        {
            Log.InternalError();
            return;
        }

        await _objectPool.PrepareAsync(item, maxCountElements, _cancellationTokenSource.Token);
    }
    
    public AppearanceItem[] GetAnimationsItems(Transform root)
    {
        var settings = _resourcesDirector.AnimationsAppearance.Items?.Where(x => x.accessible).ToList();
        if (settings == null)
        {
            Log.WriteError("Empty settings in animations type");
            return Array.Empty<AppearanceItem>();
        }
        var items = new AppearanceItem[settings.Count];

        for (var i = 0; i < settings.Count; i++)
        {
            items[i] = GetItem(settings[i].keyId, root);
        }

        return items;
    }
    
    public AppearanceItem[] GetBoxSkinItems(Transform root)
    {
        var settings = _resourcesDirector.BoxAppearance.Items?.Where(x => x.accessible).ToList();
        if (settings == null)
        {
            Log.WriteError("Empty settings in box skins type");
            return Array.Empty<AppearanceItem>();
        }
        var items = new AppearanceItem[settings.Count];

        for (var i = 0; i < settings.Count; i++)
        {
            items[i] = GetItem(settings[i].keyId, root);
        }

        return items;
    }
    
    public AppearanceItem[] GetEffectItems(Transform root)
    {
        var settings = _resourcesDirector.EffectsAppearance.Items?.Where(x => x.accessible).ToList();
        if (settings == null)
        {
            Log.WriteError("Empty settings in effects type");
            return Array.Empty<AppearanceItem>();
        }
        var items = new AppearanceItem[settings.Count];

        for (var i = 0; i < settings.Count; i++)
        {
            items[i] = GetItem(settings[i].keyId, root);
        }

        return items;
    }
    
    public AppearanceItem[] GetUserSkinItems(Transform root)
    {
        var settings = _resourcesDirector.UserSkinsAppearance.Items?.Where(x => x.accessible).ToList();
        if (settings == null)
        {
            Log.WriteError("Empty settings in user skins type");
            return Array.Empty<AppearanceItem>();
        }
        
        var items = new AppearanceItem[settings.Count];

        for (var i = 0; i < settings.Count; i++)
        {
            items[i] = GetItem(settings[i].keyId, root);
        }

        return items;
    }
    
    public AppearanceItem GetItem(string codeId, Transform root)
    {
        var prefab = GetPrefab(AddressableItemKey);
        var instance = _objectPool.Get(prefab, root);
        instance.CodeId = codeId;
        return instance;
    }
}
}