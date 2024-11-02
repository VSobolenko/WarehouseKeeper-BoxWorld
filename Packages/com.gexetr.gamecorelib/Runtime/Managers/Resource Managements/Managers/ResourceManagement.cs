using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Game.AssetContent.Managers
{
internal class ResourceManagement : IResourceManagement
{
    public T LoadAsset<T>(string key) where T : Object => Resources.Load<T>(key);

    public Task<T> LoadAssetAsync<T>(string key) where T : Object
    {
        var resourceRequest = Resources.LoadAsync<T>(key);
        var taskSource = new TaskCompletionSource<T>();
        resourceRequest.completed += _ =>
        {
            taskSource.SetResult(resourceRequest.asset as T);
        };

        return taskSource.Task;
    }

    public Task<AsyncOperationHandle<SceneInstance>> LoadSceneAsync(string key, LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true)
    {
        throw new System.NotImplementedException();
    }

    public void ClearMemory()
    {
        throw new System.NotImplementedException();
    }
}
}