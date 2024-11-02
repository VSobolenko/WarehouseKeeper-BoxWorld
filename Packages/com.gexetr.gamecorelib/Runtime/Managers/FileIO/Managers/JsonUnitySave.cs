using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.IO.Managers
{
internal class JsonUnitySave : ISaveFile, ISaveFileAsync
{
    private const string FileFormat = ".json";

    public T Read<T>(string pathToFile)
    {
        var data = File.ReadAllBytes(pathToFile + FileFormat);

        return Deserialize<T>(data);
    }

    public void Write<T>(string pathToFile, T entity, FileMode mode)
    {
        var data = Serialize<T>(entity);
        
        File.WriteAllBytes(pathToFile + FileFormat, data);
    }

    public async Task<T> ReadAsync<T>(string pathToFile)
    {
        var data = await File.ReadAllBytesAsync(pathToFile + FileFormat);

        return Deserialize<T>(data);
    }

    public async Task WriteAsync<T>(string pathToFile, T entity)
    {
        var data = Serialize(entity);
        
        await File.WriteAllBytesAsync(pathToFile + FileFormat, data);
    }

    public void Delete(string pathToFile) => File.Delete(pathToFile + FileFormat);

    public bool IsFileExist(string pathToFile) => File.Exists(pathToFile + FileFormat);

    public byte[] Serialize<T>(T entity)
    {
        var data = JsonUtility.ToJson(entity);
        return Encoding.ASCII.GetBytes(data);  
    }

    public T Deserialize<T>(byte[] bytes)
    {
        var data = Encoding.ASCII.GetString(bytes);

        return JsonUtility.FromJson<T>(data);
    }
}
}