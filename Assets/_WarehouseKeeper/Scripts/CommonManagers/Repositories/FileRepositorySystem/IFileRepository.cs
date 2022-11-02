using System.IO;

namespace WarehouseKeeper.Repositories.FileRepositorySystem
{
internal interface IFileRepository
{
    T Read<T>(string pathToFile);
    void Write<T>(string pathToFile, T entity, FileMode mode);
    void Delete(string pathToFile);
    bool IsFileExist(string pathToFile);
    
    byte[] Serialize<T>(T entity);
    T Deserialize<T>(byte[] bytes);
}
}