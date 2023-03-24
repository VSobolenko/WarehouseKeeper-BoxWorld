using System.Reflection;
using NUnit.Framework;
using WarehouseKeeper.Directors;

namespace WarehouseKeeper.Test
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
            Assert.IsNotNull(value);
        }
    }
}
}