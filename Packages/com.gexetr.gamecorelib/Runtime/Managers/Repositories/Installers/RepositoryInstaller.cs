using Game.IO;
using Game.Repositories.Managers;

namespace Game.Repositories.Installers
{
public class RepositoryInstaller
{
    public static IRepository<T> File<T>(string path, ISaveFile fileSaver) where T : class, IHasBasicId =>
        new FileRepositoryManager<T>(path, fileSaver);

    public static IRepository<T> StaticResources<T>(string path, ISaveFile fileSaver) where T : class, IHasBasicId =>
        new StaticResourcesRepositoryManager<T>(path, fileSaver);
}
}