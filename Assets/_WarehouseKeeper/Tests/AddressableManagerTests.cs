using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using WarehouseKeeper.AssetContent;
using WarehouseKeeper.AssetContent.Managers;

namespace WarehouseKeeper.Test
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
        IAddressablesManager addressablesManager = new AddressablesManager();
        
        // Act
        var gameObject = addressablesManager.LoadAsset<GameObject>(GameObjectAssetTestKey);
        
        // Assert
        Assert.IsTrue(gameObject != null);
    }
    
    [Test]
    public void LoadAsset_WhenAssetNotExist_ShouldReturnNull()
    {
        // Arrange
        IAddressablesManager addressablesManager = new AddressablesManager();
        
        // Act
        var gameObject = addressablesManager.LoadAsset<GameObject>(NonExistentAssetKey);
        
        // Assert
        Assert.IsNull(gameObject);
    }
    
    [Test]
    public void LoadAsset_WhenStringIsNullOrEmpty_ShouldReturnNull()
    {
        // Arrange
        IAddressablesManager addressablesManager = new AddressablesManager();
        
        // Act
        var emptyStringGameObject = addressablesManager.LoadAsset<GameObject>("");
        var nullStringGameObject = addressablesManager.LoadAsset<GameObject>(null);
        
        // Assert
        Assert.IsNull(emptyStringGameObject);
        Assert.IsNull(nullStringGameObject);
    }
    
    [Test]
    public async Task LoadAssetAsync_WhenAssetExist_ShouldReturnNotNull()
    {
        // Arrange
        IAddressablesManager addressablesManager = new AddressablesManager();
        
        // Act
        var gameObject = await addressablesManager.LoadAssetAsync<GameObject>(GameObjectAssetTestKey);
        
        // Assert
        Assert.IsTrue(gameObject != null);
    }
    
    [Test]
    public async Task LoadAssetAsync_WhenAssetNonExist_ShouldReturnNull()
    {
        // Arrange
        IAddressablesManager addressablesManager = new AddressablesManager();
        
        // Act
        var gameObject = await addressablesManager.LoadAssetAsync<GameObject>(NonExistentAssetKey);
        
        // Assert
        Assert.IsTrue(gameObject == null);
    }
    
    [Test]
    public async Task LoadAssetAsync_WhenStringIsNullOrEmpty_ShouldReturnNull()
    {
        // Arrange
        IAddressablesManager addressablesManager = new AddressablesManager();
        
        // Act
        var emptyStringGameObject = await addressablesManager.LoadAssetAsync<GameObject>("");
        var nullStringGameObject = await addressablesManager.LoadAssetAsync<GameObject>(null);
        
        // Assert
        Assert.IsNull(emptyStringGameObject);
        Assert.IsNull(nullStringGameObject);
    }
    
    [Test]
    public async Task LoadSceneAsync_WhenAssetExist_ShouldReturnHandle()
    {
        // Arrange
        IAddressablesManager addressablesManager = new AddressablesManager();
        
        // Act
        var sceneHandle = await addressablesManager.LoadSceneAsync(SceneAssetTestKey);
        var task = await sceneHandle.Task;
        var scene = task.Scene;

        // Assert
        Assert.IsTrue(scene != null);
    }
    
    [Test]
    public async Task LoadSceneAsync_WhenAssetNonExist_ShouldReturnDefaultHandle()
    {
        // Arrange
        IAddressablesManager addressablesManager = new AddressablesManager();
        
        // Act
        var handle = await addressablesManager.LoadSceneAsync(NonExistentAssetKey);
        
        // Assert
        Assert.IsFalse(handle.IsValid());
    }
    
    [Test]
    public async Task LoadSceneAsync_WhenStringIsNullOrEmpty_ShouldReturnDefaultHandle()
    {
        // Arrange
        IAddressablesManager addressablesManager = new AddressablesManager();
        
        // Act
        var emptyStringGameObject = await addressablesManager.LoadSceneAsync("");
        var nullStringGameObject = await addressablesManager.LoadSceneAsync(null);
        
        // Assert
        Assert.IsFalse(emptyStringGameObject.IsValid());
        Assert.IsFalse(nullStringGameObject.IsValid());
    }
}
}