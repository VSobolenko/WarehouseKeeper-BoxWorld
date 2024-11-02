using System.Threading.Tasks;
using Game.AssetContent;
using Game.AssetContent.Managers;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameTests.AssetContent
{
[TestFixture]
public class AddressableManagerTests
{
    private const string GameObjectAssetTestKey = "AddressableTestGameObject";
    private const string SceneAssetTestKey = "AddressableTestScene";
    private const string NonExistentAssetKey = "152e586e-e610-4fa5-a707-652c6cab564f";

    [Test]
    public void LoadAsset_WhenAssetExist_ShouldReturnNotNull()
    {
        // Arrange
        IResourceManagement resourceManagement = new AddressablesManager();
        
        // Act
        var gameObject = resourceManagement.LoadAsset<GameObject>(GameObjectAssetTestKey);
        
        // Assert
        Assert.IsTrue(gameObject != null);
    }
    
    [Test]
    public void LoadAsset_WhenAssetNotExist_ShouldReturnNull()
    {
        // Arrange
        IResourceManagement resourceManagement = new AddressablesManager();
        
        // Act
        var gameObject = resourceManagement.LoadAsset<GameObject>(NonExistentAssetKey);
        
        // Assert
        Assert.IsNull(gameObject);
    }
    
    [Test]
    public void LoadAsset_WhenStringIsNullOrEmpty_ShouldReturnNull()
    {
        // Arrange
        IResourceManagement resourceManagement = new AddressablesManager();
        
        // Act
        var emptyStringGameObject = resourceManagement.LoadAsset<GameObject>("");
        var nullStringGameObject = resourceManagement.LoadAsset<GameObject>(null);
        
        // Assert
        Assert.IsNull(emptyStringGameObject);
        Assert.IsNull(nullStringGameObject);
    }
    
    [Test]
    public async Task LoadAssetAsync_WhenAssetExist_ShouldReturnNotNull()
    {
        // Arrange
        IResourceManagement resourceManagement = new AddressablesManager();
        
        // Act
        var gameObject = await resourceManagement.LoadAssetAsync<GameObject>(GameObjectAssetTestKey);
        
        // Assert
        Assert.IsTrue(gameObject != null);
    }
    
    [Test]
    public async Task LoadAssetAsync_WhenAssetNonExist_ShouldReturnNull()
    {
        // Arrange
        IResourceManagement resourceManagement = new AddressablesManager();
        
        // Act
        var gameObject = await resourceManagement.LoadAssetAsync<GameObject>(NonExistentAssetKey);
        
        // Assert
        Assert.IsTrue(gameObject == null);
    }
    
    [Test]
    public async Task LoadAssetAsync_WhenStringIsNullOrEmpty_ShouldReturnNull()
    {
        // Arrange
        IResourceManagement resourceManagement = new AddressablesManager();
        
        // Act
        var emptyStringGameObject = await resourceManagement.LoadAssetAsync<GameObject>("");
        var nullStringGameObject = await resourceManagement.LoadAssetAsync<GameObject>(null);
        
        // Assert
        Assert.IsNull(emptyStringGameObject);
        Assert.IsNull(nullStringGameObject);
    }
    
    [Test]
    public async Task LoadSceneAsync_WhenAssetExist_ShouldReturnHandle()
    {
        // Arrange
        IResourceManagement resourceManagement = new AddressablesManager();
        
        // Act
        var sceneHandle = await resourceManagement.LoadSceneAsync(SceneAssetTestKey, LoadSceneMode.Additive);
        var task = await sceneHandle.Task;
        var scene = task.Scene;

        // Assert
        Assert.IsTrue(scene != null);
    }
    
    [Test]
    public async Task LoadSceneAsync_WhenAssetNonExist_ShouldReturnDefaultHandle()
    {
        // Arrange
        IResourceManagement resourceManagement = new AddressablesManager();
        
        // Act
        var handle = await resourceManagement.LoadSceneAsync(NonExistentAssetKey);
        
        // Assert
        Assert.IsFalse(handle.IsValid());
    }
    
    [Test]
    public async Task LoadSceneAsync_WhenStringIsNullOrEmpty_ShouldReturnDefaultHandle()
    {
        // Arrange
        IResourceManagement resourceManagement = new AddressablesManager();
        
        // Act
        var emptyStringGameObject = await resourceManagement.LoadSceneAsync("");
        var nullStringGameObject = await resourceManagement.LoadSceneAsync(null);
        
        // Assert
        Assert.IsFalse(emptyStringGameObject.IsValid());
        Assert.IsFalse(nullStringGameObject.IsValid());
    }
}
}