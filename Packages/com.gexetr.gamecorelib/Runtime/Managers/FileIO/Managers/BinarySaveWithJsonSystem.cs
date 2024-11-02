using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Game.IO.Managers
{
internal class BinarySaveWithJsonSystem : ISaveFile, ISaveFileAsync
{
    private const string FileFormat = ".bat";
    private readonly ISaveFile _json;

    public BinarySaveWithJsonSystem(ISaveFile json)
    {
        _json = json;
    }

    public T Read<T>(string pathToFile)
    {
        using (Stream stream = File.Open(pathToFile + FileFormat, FileMode.Open, FileAccess.Read))
        {
            var binaryFormatter = new BinaryFormatter();

            var bytes = (byte[]) binaryFormatter.Deserialize(stream);

            return _json.Deserialize<T>(bytes);
        }
    }

    public void Write<T>(string pathToFile, T entity, FileMode mode)
    {
        var bytesStringData = _json.Serialize(entity);
        using (Stream stream = File.Open(pathToFile + FileFormat, mode, FileAccess.Write))
        {
            var binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(stream, bytesStringData);
        }
    }

    public async Task<T> ReadAsync<T>(string pathToFile)
    {
        var bytes = await File.ReadAllBytesAsync(pathToFile + FileFormat);
        var stringData =  Deserialize<string>(bytes);

        return _json.Deserialize<T>(Encoding.ASCII.GetBytes(stringData));
    }

    public async Task WriteAsync<T>(string pathToFile, T entity)
    {
        var stringDta = _json.Serialize(entity);
        var bytes = Serialize(stringDta);
        await File.WriteAllBytesAsync(pathToFile + FileFormat, bytes);
    }

    public void Delete(string pathToFile) => File.Delete(pathToFile + FileFormat);

    public bool IsFileExist(string pathToFile) => File.Exists(pathToFile + FileFormat);

    public byte[] Serialize<T>(T entity)
    {
        var stringData = _json.Serialize(entity);
        using (var stream = new MemoryStream())
        {
            new BinaryFormatter().Serialize(stream, stringData);
            return stream.ToArray();
        }
    }

    public T Deserialize<T>(byte[] bytes)
    {
        using (var stream = new MemoryStream(bytes))
        {
            var deserializeBytes = (byte[]) new BinaryFormatter().Deserialize(stream);

            return _json.Deserialize<T>(deserializeBytes);
        }
    }
}
}