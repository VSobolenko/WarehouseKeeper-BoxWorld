using System.Threading.Tasks;
using Game;
using Game.AssetContent;
using UnityEngine;

namespace WarehouseKeeper.UI.Windows
{
internal class PrefabProviderByAddress<T> where T : class
{
    protected readonly IAddressablesManager addressablesManager;

    private T _cachedPrefab;

    public PrefabProviderByAddress(IAddressablesManager addressablesManager)
    {
        this.addressablesManager = addressablesManager;
    }

    protected async Task<T> GetPrefabAsync(string addressableKey)
    {
        if (_cachedPrefab != null)
            return _cachedPrefab;

        var prefab = await addressablesManager.LoadAssetAsync<GameObject>(addressableKey);
        
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

        _cachedPrefab = levelSelectionItem;
        return levelSelectionItem;
    }
    
    protected T GetPrefab(string addressableKey)
    {
        if (_cachedPrefab != null)
            return _cachedPrefab;
        
        var prefab = addressablesManager.LoadAsset<GameObject>(addressableKey);
        
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