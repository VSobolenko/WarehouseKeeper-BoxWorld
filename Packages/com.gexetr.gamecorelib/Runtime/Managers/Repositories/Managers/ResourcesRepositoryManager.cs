using System.Collections.Generic;
using UnityEngine;

namespace Game.Repositories.Managers
{
internal class ResourcesRepositoryManager<T> : BaseRepositoryManager<T>, IRepository<T> where T : Object, IHasBasicId
{
    private readonly string _path;

    public ResourcesRepositoryManager(string path)
    {
        _path = path;
        
        if (typeof(T).IsSubclassOf(typeof(UnityEngine.Object)) == false)
            throw new System.NotSupportedException("Supported only unity objects in this repository");
    }

    public T Read(int id)
    {
        var fileName = GetEntityUniqueName(id);
        return Resources.Load<T>(_path + fileName);
    }

    public IEnumerable<T> ReadAll() => Resources.LoadAll<T>(_path);

    public int Create(T entity) => throw new System.NotSupportedException("Create in resources not supported");
    
    public void CreateById(T entity) => throw new System.NotSupportedException("Create in resources not supported");

    public void Update(T entity) => throw new System.NotSupportedException("Update in resources not supported");

    public void Delete(T entity) => throw new System.NotSupportedException("Delete in resources not supported");
}
}