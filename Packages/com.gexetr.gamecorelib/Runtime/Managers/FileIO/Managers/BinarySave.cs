using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace Game.IO.Managers
{
internal class BinarySave : ISaveFile, ISaveFileAsync
{
    private const string FileFormat = ".txt";
    
    public T Read<T>(string pathToFile)
    {
        using (Stream stream = File.Open(pathToFile + FileFormat, FileMode.Open, FileAccess.Read))
        {
            var binaryFormatter = new BinaryFormatter();

            return (T) binaryFormatter.Deserialize(stream);
        }
    }

    public void Write<T>(string pathToFile, T entity, FileMode mode)
    {
        using (Stream stream = File.Open(pathToFile + FileFormat, mode, FileAccess.Write))
        {
            var binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(stream, entity);
        }
    }

    public async Task<T> ReadAsync<T>(string pathToFile)
    {
        var bytes = await File.ReadAllBytesAsync(pathToFile + FileFormat);

        return Deserialize<T>(bytes);
    }

    public async Task WriteAsync<T>(string pathToFile, T entity)
    {
        var bytes = Serialize(entity);
        await File.WriteAllBytesAsync(pathToFile + FileFormat, bytes);
    }

    public void Delete(string pathToFile) => File.Delete(pathToFile + FileFormat);

    public bool IsFileExist(string pathToFile) => File.Exists(pathToFile + FileFormat);

    public byte[] Serialize<T>(T entity)
    {
        using (var stream = new MemoryStream())
        {
            new BinaryFormatter().Serialize(stream, entity);
            return stream.ToArray();
        }
    }

    public T Deserialize<T>(byte[] bytes)
    {
        using (var stream = new MemoryStream(bytes))
        {
            return (T)new BinaryFormatter().Deserialize(stream);
        }
    }
}
}