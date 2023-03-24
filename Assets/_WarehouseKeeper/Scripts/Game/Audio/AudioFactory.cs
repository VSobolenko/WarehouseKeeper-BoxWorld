using System;
using System.Threading;
using System.Threading.Tasks;
using WarehouseKeeper.AssetContent;
using WarehouseKeeper.Audio;
using WarehouseKeeper.Extension;
using WarehouseKeeper.Pools;
using WarehouseKeeper.UI.Windows;
using Zenject;

namespace WarehouseKeeper.Directors.Game.Audio
{
internal class AudioFactory : PrefabProviderByAddress<Source>, IInitializable, IDisposable
{
    private readonly IObjectPoolManager _objectPool;
    private const string AddressableItemKey = "AudioSource";
    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    public AudioFactory(IAddressablesManager addressablesManager, IObjectPoolManager objectPool) : base(addressablesManager)
    {
        _objectPool = objectPool;
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
        var item = await GetPrefabAsync(AddressableItemKey);
        if (item == null)
        {
            Log.InternalError();
            return;
        }

        await _objectPool.PrepareAsync(item, 2, _cancellationTokenSource.Token);
    }

    public Source GetSource()
    {
        var prefab = GetPrefab(AddressableItemKey);

        return _objectPool.Get(prefab);
    }
}
}