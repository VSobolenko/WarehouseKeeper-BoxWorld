using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Game.AssetContent
{
public interface IResourceManagement
{
    T LoadAsset<T>(string key) where T : Object;

    Task<T>  LoadAssetAsync<T>(string key) where T : Object;

    Task<AsyncOperationHandle<SceneInstance>> LoadSceneAsync(string key, LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true);

    void ClearMemory();
}
}