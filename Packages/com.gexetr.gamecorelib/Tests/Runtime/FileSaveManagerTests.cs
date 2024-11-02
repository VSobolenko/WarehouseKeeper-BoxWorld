using Game.Factories;
using Game.Factories.Managers;
using NUnit.Framework;
using UnityEngine;
using Zenject;

namespace GameTests
{
[TestFixture]
public class FileSaveManagerTests
{
    [Test]
    public void InstantiateEmpty_CreateNewEmptyGameObject_ShouldReturnNewEmptyGameObject()
    {
        // Arrange
        var container = new DiContainer();
        IFactoryGameObjects factoryGameObjects = new DependencyInjectionFactory(container);
        
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