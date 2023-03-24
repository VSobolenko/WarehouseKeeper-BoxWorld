using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using WarehouseKeeper.AssetContent;
using WarehouseKeeper.Extension;
using WarehouseKeeper.Levels;
using WarehouseKeeper.Pools;
using Zenject;

namespace WarehouseKeeper.UI.Windows.LevelSelections
{
internal class LevelSelectionItemFactory : PrefabProviderByAddress<LevelSelectionItem>, IInitializable, IDisposable
{
    private const string AddressableItemKey = "LevelSelectionItem";

    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly IObjectPoolManager _objectPool;
    private readonly LevelRepositoryDirector _levelRepository;

    public LevelSelectionItemFactory(IAddressablesManager addressablesManager, 
                                     IObjectPoolManager objectPool, 
                                     LevelRepositoryDirector levelRepository) : base(addressablesManager)
    {
        _objectPool = objectPool;
        _levelRepository = levelRepository;
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
        var countSettings = _levelRepository.GetLevelsSettings().Count();
        var item = await GetPrefabAsync(AddressableItemKey);
        if (item == null)
        {
            Log.InternalError();
            return;
        }

        await _objectPool.PrepareAsync(item, countSettings, _cancellationTokenSource.Token);
    }

    // public async Task<LevelSelectionItem[]> GetLevelItemsAsync()
    // {
    //     return null;
    // }
    
    public LevelSelectionItem[] GetItems(Transform root)
    {
        var settings = _levelRepository.GetLevelsSettings().OrderBy(x => x.Id).ToList();
        var items = new LevelSelectionItem[settings.Count];

        for (var i = 0; i < settings.Count; i++)
            items[i] = GetItem(settings[i].Id, root);

        return items;
    }

    public LevelSelectionItem GetItem(int id, Transform root)
    {
        var prefab = GetPrefab(AddressableItemKey);
        var instance = _objectPool.Get(prefab, root);
        instance.transform.localScale = prefab.transform.localScale;
        instance.LevelId = id;
        UpdateItemState(instance);
        return instance;
    }

    public void UpdateItemState(LevelSelectionItem item)
    {
        if (item == null)
        {
            Log.InternalError();
            return;
        }

        var id = item.LevelId;
        var data = _levelRepository.GetLevelData(id);
        item.LevelPrice = 150;
        if (data == null)
        {
            item.SetupLockedState(150, _levelRepository.GetLevelData(id - 1) != null);

            return;
        }
        item.SetupUnlockedState(data);
    }
}
}