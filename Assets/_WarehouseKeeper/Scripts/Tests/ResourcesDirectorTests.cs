using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using WarehouseKeeper.Directors;

namespace GameTests
{
[TestFixture]
public class ResourcesDirectorTests
{
    [Test]
    public void Properties_LoadResources_ShouldReturnReferenceToResources()
    {
        // Arrange
        var resourcesDirector = new ResourcesDirector();
        
        // Act
        var properties = resourcesDirector.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        
        // Assert
        foreach (var propertyInfo in properties)
        {
            var value = propertyInfo.GetValue(resourcesDirector);
            if (value == null)
                Debug.Log($"Null properties: {propertyInfo.Name}");

            Assert.IsNotNull(value);
        }
    }
}
}