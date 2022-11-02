using NUnit.Framework;
using UnityEngine;
using WarehouseKeeper.Factories;
using WarehouseKeeper.Factories.Managers;
using Zenject;

namespace WarehouseKeeper.Test
{
[TestFixture]
public class FileSaveManagerTests
{
    [Test]
    public void InstantiateEmpty_CreateNewEmptyGameObject_ShouldReturnNewEmptyGameObject()
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
}
}