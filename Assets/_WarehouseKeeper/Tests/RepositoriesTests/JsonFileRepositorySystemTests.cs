using System.IO;
using System.Reflection;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine;
using WarehouseKeeper.Extension;
using WarehouseKeeper.Repositories.FileRepositorySystem;
using WarehouseKeeper.Test.TestingElements;

namespace WarehouseKeeper.Test.Repository
{
[TestFixture]
public class JsonFileSystemTests
{
    private const string FileFormat = ".json";
    private const string TestFolder = "e38b5a63-JsonTestedFiles-9819-4425-aa1c-c91e3d598839";

    private TestClassWithUnityVectorAndQuaternion ReadableClass => new TestClassWithUnityVectorAndQuaternion
        {id = 7, name = "Player", position = Vector3.up, rotation = Quaternion.identity};
    
    private string _testFolderPath;
    private bool _deleteTestFolder = true;
    
    private string _pathToReadFile;
    private string _pathToWriteFile;
    private string _pathToDeleteFile;
    private string _pathToExistFile;
    private string _pathToNotExistFile;
    
    [OneTimeSetUp]
    public void Init()
    {
        var activeExeLocation = Assembly.GetExecutingAssembly().Location;
        _testFolderPath = Path.GetDirectoryName(activeExeLocation) + $@"\{TestFolder}\";

        if (Directory.Exists(_testFolderPath) == false)
            Directory.CreateDirectory(_testFolderPath);
        else
        {
            Log.WriteWarning("The folder for temporary files already exists. An empty folder is required!");
            _deleteTestFolder = false;
        }
        
        // Read file
        _pathToReadFile = Path.Combine(_testFolderPath + "ReadFile");
        var savableData = GetSavedData(ReadableClass);
        File.WriteAllText(_pathToReadFile + FileFormat, savableData);
        
        // Write file
        _pathToWriteFile = Path.Combine(_testFolderPath + "WriteFile");
        
        // Delete file
        _pathToDeleteFile = Path.Combine(_testFolderPath + "DeleteFile");
        if (File.Exists(_pathToDeleteFile + FileFormat) == false)
            using (File.Create(_pathToDeleteFile + FileFormat))
            { }

        // Exists file
        _pathToExistFile = Path.Combine(_testFolderPath + "ExistFile");
        if (File.Exists(_pathToExistFile + FileFormat) == false)
            using (File.Create(_pathToExistFile + FileFormat))
            { }

        // Not exists file
        _pathToNotExistFile = _testFolderPath + "NotExistFile" + FileFormat;
    }
        
    [OneTimeTearDown]
    public void Revert()
    {
        if (_deleteTestFolder)
            Directory.Delete(_testFolderPath, true);
    }
    
    private string GetSavedData(object obj)
    {
        return JsonUtility.ToJson(obj);
    }

    [Test]
    public void Read_DataFromJsonFile_ShouldReturnNewClassWithData()
    {
        // Arrange
        IFileRepository fileRepository = new JsonFileRepositorySystem();
        
        // Act
        var data = fileRepository.Read<TestClassWithUnityVectorAndQuaternion>(_pathToReadFile);
        
        // Assert
        data.Should().BeEquivalentTo(ReadableClass);
    }
    
    [Test]
    public void Write_DataToFile_ShouldSaveNewClassDataToFile()
    {
        // Arrange
        IFileRepository fileRepository = new JsonFileRepositorySystem();
        var testData = new TestClassWithUnityVectorAndQuaternion
            {id = 7, name = "Player", position = Vector3.up, rotation = Quaternion.identity};
        var savedData = GetSavedData(testData);
        
        // Act
        fileRepository.Write(_pathToWriteFile, testData, FileMode.Create);
        var fileData = File.ReadAllText(_pathToWriteFile + FileFormat);
    
        // Assert
        fileData.Should().BeEquivalentTo(savedData);
    }

    [Test]
    public void Write_DataToFileWithData_ShouldSaveNewWriteData()
    {
        // Arrange
        IFileRepository fileRepository = new JsonFileRepositorySystem();
        var firstTestData = new TestClassWithUnityVectorAndQuaternion
            {id = 7, name = "Player", position = Vector3.up, rotation = Quaternion.identity};
        var secondTestData = new TestClassWithUnityVectorAndQuaternion
            {id = 8, name = "Enemy", position = Vector3.one, rotation = Quaternion.identity};
        var firstSaveData = GetSavedData(firstTestData);
        var secondSaveData = GetSavedData(secondTestData);
        
        // Act
        fileRepository.Write(_pathToWriteFile, firstTestData, FileMode.Create);
        fileRepository.Write(_pathToWriteFile, secondTestData, FileMode.Create);
        var fileData = File.ReadAllText(_pathToWriteFile + FileFormat);
    
        // Assert
        fileData.Should().BeEquivalentTo(secondSaveData);
        fileData.Should().NotBeEquivalentTo(firstSaveData);
    }
    
    [Test]
    public void Delete_ExistFile_ShouldFolderWithoutFile()
    {
        // Arrange
        IFileRepository fileRepository = new JsonFileRepositorySystem();
        
        // Act
        fileRepository.Delete(_pathToDeleteFile);
    
        // Assert
        Assert.IsFalse(File.Exists(_pathToDeleteFile + FileFormat));
    }
    
    [Test]
    public void Exist_CheckAvailableFile_ShouldReturnTrue()
    {
        // Arrange
        IFileRepository fileRepository = new JsonFileRepositorySystem();
        
        // Act
        var fileExist = fileRepository.IsFileExist(_pathToExistFile);
    
        // Assert
        Assert.IsTrue(fileExist);
    }
    
    [Test]
    public void Exist_CheckNotAvailableFile_ShouldReturnFalse()
    {
        // Arrange
        IFileRepository fileRepository = new JsonFileRepositorySystem();
        
        // Act
        var fileExist = fileRepository.IsFileExist(_pathToNotExistFile);
    
        // Assert
        Assert.IsFalse(fileExist);
    }
    
    [Test]
    public void Serialize_ClassWithData_ShouldReturnSerializeToJsonString()
    {
        // Arrange
        IFileRepository fileRepository = new JsonFileRepositorySystem();
        var testData = new TestClassWithUnityVectorAndQuaternion
            {id = 7, name = "Player", position = Vector3.up, rotation = Quaternion.identity};
        var serializeData = GetSavedData(testData);
        var serializeBytesData = Encoding.ASCII.GetBytes(serializeData);
        
        // Act
        var serializableData = fileRepository.Serialize(testData);
    
        // Assert
        serializableData.Should().BeEquivalentTo(serializeBytesData);
    }
    
    [Test]
    public void Deserialize_JsonString_ShouldReturnNewClassWithData()
    {
        // Arrange
        IFileRepository fileRepository = new JsonFileRepositorySystem();
        var testData = new TestClassWithUnityVectorAndQuaternion
            {id = 7, name = "Player", position = Vector3.up, rotation = Quaternion.identity};
        var serializeData = GetSavedData(testData);
        var serializeBytesData = Encoding.ASCII.GetBytes(serializeData);
        
        // Act
        var deserializableClass = fileRepository.Deserialize<TestClassWithUnityVectorAndQuaternion>(serializeBytesData);
    
        // Assert
        deserializableClass.Should().BeEquivalentTo(testData);
    }
}
}