using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using WarehouseKeeper.Extension;

namespace WarehouseKeeper.AssetContent.Managers
{
internal class AddressablesManager : System.IDisposable, IAddressablesManager
{
    private readonly Dictionary<string, AsyncOperationHandle> _loadedHandlers;

    public AddressablesManager()
    {
        _loadedHandlers = new Dictionary<string, AsyncOperationHandle>();

        Addressables.InitializeAsync();
    }

    public T LoadAsset<T>(string key) where T : Object
    {
        if (string.IsNullOrEmpty(key))
        {
            Log.WriteWarning("Unable to load null or empty key");

            return null;
        }

        if (_loadedHandlers.ContainsKey(key))
        {
            var existHandle = _loadedHandlers[key].Convert<T>();

            return existHandle.IsDone == false ? existHandle.WaitForCompletion() : existHandle.Result;
        }

        if (IsKeyExist(key) == false)
        {
            Log.WriteWarning($"Asset key not found: \"{key}\"");

            return null;
        }

        var handle = Addressables.LoadAssetAsync<T>(key);
        var asset = handle.WaitForCompletion();

        if (IsHandleCompleteSuccess(ref handle) == false)
        {
            Log.WriteError($"Asset loading error: Status={handle.Status}; IsDone={handle.IsDone}");

            return default;
        }
        
        _loadedHandlers.Add(key, handle);

        return asset;
    }

    public async Task<T> LoadAssetAsync<T>(string key) where T : Object
    {
        if (string.IsNullOrEmpty(key))
        {
            Log.WriteWarning("Unable to load null or empty key");

            return null;
        }

        if (_loadedHandlers.ContainsKey(key)) 
            return _loadedHandlers[key].Convert<T>().Result;

        if (await IsKeyExistAsync(key) == false)
        {
            Log.WriteWarning($"Asset key not found: \"{key}\"");

            return null;
        }

        var handle = Addressables.LoadAssetAsync<T>(key);
        await handle.Task;

        if (IsHandleCompleteSuccess(ref handle) == false)
        {
            Log.WriteError($"Asset loading error: Status={handle.Status}; IsDone={handle.IsDone}");

            return default;
        }

        _loadedHandlers.Add(key, handle);

        return handle.Result;
    }

    public async Task<AsyncOperationHandle<SceneInstance>> LoadSceneAsync(
        string key, LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true)
    {
        if (string.IsNullOrEmpty(key))
        {
            Log.WriteWarning("Unable to load empty key");

            return default;
        }

        if (await IsKeyExistAsync(key) == false)
        {
            Log.WriteWarning($"Asset key not found: \"{key}\"");

            return default;
        }

        var handle = Addressables.LoadSceneAsync(key, loadMode, activateOnLoad);

        return handle;
    }

    // ToDo: test this method
    public void ClearMemory()
    {
        foreach (var (key, handler) in _loadedHandlers)
        {
            if (handler.IsValid())
                Addressables.Release(handler);
            else
                Log.WriteError($"handler in not valid: {key}");
        }

        _loadedHandlers.Clear();
    }

    // ToDo: what is it
    public void Dispose()
    {
    }

    private static async Task<bool> IsKeyExistAsync(string key)
    {
        var handle = Addressables.LoadResourceLocationsAsync(key);
        var result = await handle.Task;

        if (IsHandleCompleteSuccess(ref handle) == false)
        {
            Log.WriteError($"Asset loading error: Status={handle.Status}; IsDone={handle.IsDone}");

            return default;
        }

        return result.Count > 0;
    }

    private static bool IsKeyExist(string key)
    {
        var handle = Addressables.LoadResourceLocationsAsync(key);
        var result = handle.WaitForCompletion();

        if (IsHandleCompleteSuccess(ref handle) == false)
        {
            Log.WriteError($"Asset loading error: Status={handle.Status}; IsDone={handle.IsDone}");

            return default;
        }

        return result.Count > 0;
    }
    
    private static bool IsHandleCompleteSuccess<T>(ref AsyncOperationHandle<T> handle) => 
        handle.Status == AsyncOperationStatus.Succeeded && handle.IsDone && handle.IsValid();
}
}