using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using WarehouseKeeper.Factories;
using WarehouseKeeper.Factories.Managers;
using WarehouseKeeper.Test.TestingElements;
using Zenject;

namespace WarehouseKeeper.Test
{
[TestFixture]
public class GameObjectFactoryTest
{
    private const string PathToTestPrefab = "Assets/_WarehouseKeeper/Sandbox/Cube.prefab";
    
    
    [Test]
    public void InstantiateEmpty_EmptyGameObject_ShouldReturnNewEmptyGameObject()
    {
        // Arrange
        var container = new DiContainer();
        IFactoryGameObjects factoryGameObjects = new GameObjectsFactory(container);
        
        // Act
        var gameObject = factoryGameObjects.InstantiateEmpty();
        var gameObjectComponents = gameObject.GetComponents(typeof(Component));
        
        // Assert
        Assert.IsTrue(gameObject != null);
        Assert.IsTrue(gameObject is { });
        Assert.IsTrue(gameObject.transform.childCount == 0);
        Assert.IsTrue(gameObjectComponents.Length == 1); // Transform = 1
        Assert.IsTrue(gameObjectComponents[0].GetType() == typeof(Transform));
    }
    
    [Test]
    public void InstantiateEmpty_GameObjectWithRectTransformCanvasRendererButton_ShouldReturnNewEmptyGameObjectWith3Components()
    {
        // Arrange
        var container = new DiContainer();
        IFactoryGameObjects factoryGameObjects = new GameObjectsFactory(container);
        
        // Act
        var gameObject = factoryGameObjects.InstantiateEmpty(typeof(RectTransform), typeof(CanvasRenderer), typeof(Button));
        var gameObjectComponents = gameObject.GetComponents(typeof(Component));
        
        // Assert
        Assert.IsTrue(gameObject != null);
        Assert.IsTrue(gameObject.transform.childCount == 0);
        Assert.IsTrue(gameObjectComponents.Length == 3); // RectTransform + CanvasRenderer + Button = 3
        Assert.IsTrue(gameObjectComponents[0] is RectTransform);
        Assert.IsTrue(gameObjectComponents[1] is CanvasRenderer);
        Assert.IsTrue(gameObjectComponents[2] is Button);
    }
    
    [Test]
    public void Instantiate_Prefab_ShouldReturnNewGameObject()
    {
        // Arrange
        var container = new DiContainer();
        IFactoryGameObjects factoryGameObjects = new GameObjectsFactory(container);
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PathToTestPrefab);
        
        // Act
        var gameObject = factoryGameObjects.InstantiatePrefab(prefab);
        
        // Assert
        Assert.IsTrue(gameObject != null);
    }
    
    [Test]
    public void Instantiate_PrefabWithComponent_ShouldReturnNewGameObjectWithComponent()
    {
        // Arrange
        var container = new DiContainer();
        IFactoryGameObjects factoryGameObjects = new GameObjectsFactory(container);
        var prefab = AssetDatabase.LoadAssetAtPath<TestMonoBehaviour>(PathToTestPrefab);
        
        // Act
        var gameObject = factoryGameObjects.Instantiate<TestMonoBehaviour>(prefab);
        
        // Assert
        Assert.IsTrue(gameObject != null);
        Assert.IsTrue(gameObject.GetType() == typeof(TestMonoBehaviour));
    }
    
    [Test]
    public void InstantiateAndAddNewComponent_GameObjectWithNewComponent_ShouldReturnNewGameObjectWithComponent()
    {
        // Arrange
        var container = new DiContainer();
        IFactoryGameObjects factoryGameObjects = new GameObjectsFactory(container);
        
        // Act
        var gameObject = factoryGameObjects.InstantiateAndAddNewComponent<TestMonoBehaviour>();
        var gameObjectComponents = gameObject.GetComponents(typeof(Component));
        
        // Assert
        Assert.IsTrue(gameObject != null);
        Assert.IsTrue(gameObject.GetType() == typeof(TestMonoBehaviour));
        Assert.IsTrue(gameObject.GetComponent<TestMonoBehaviour>() != null);
        Assert.IsTrue(gameObject.transform.childCount == 0);
        Assert.IsTrue(gameObjectComponents.Length == 2); // Transform + TestMonoBehaviour = 2
    }
    
    [Test]
    public void AddComponent_ToExistingGameObject_ShouldReturnNewGameObjectWithNewComponent()
    {
        // Arrange
        var container = new DiContainer();
        IFactoryGameObjects factoryGameObjects = new GameObjectsFactory(container);
        var gameObject = new GameObject("TestingGameObject");

        // Act
        var newMonoBehavior = factoryGameObjects.AddComponent<NewMonoBehavior>(gameObject);
        
        // Assert
        Assert.IsTrue(gameObject != null);
        Assert.IsTrue(gameObject.GetComponent<NewMonoBehavior>() != null);
        Assert.IsTrue(newMonoBehavior.GetType() == typeof(NewMonoBehavior));
    }
}
}