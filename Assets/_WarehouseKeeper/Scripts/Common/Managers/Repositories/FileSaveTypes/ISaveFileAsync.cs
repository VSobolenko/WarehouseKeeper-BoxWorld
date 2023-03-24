using System.Threading.Tasks;

namespace WarehouseKeeper.Repositories.FileSave
{
public interface ISaveFileAsync
{
    //ToDo: test async methods
    Task<T> ReadAsync<T>(string pathToFile);
    Task WriteAsync<T>(string pathToFile, T entity);
    void Delete(string pathToFile);
    bool IsFileExist(string pathToFile);

    byte[] Serialize<T>(T entity);
    T Deserialize<T>(byte[] bytes);
}
}