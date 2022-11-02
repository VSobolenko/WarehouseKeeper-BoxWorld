using System.Collections.Generic;
using UnityEngine;
using WarehouseKeeper.Repositories;

namespace WarehouseKeeper.Managers.Repositories
{
public class ResourcesManager<T> : BaseSaveManager<T>, IRepository<T> where T : Object, IHasBasicId
{
    private readonly string _path;

    public ResourcesManager(string path)
    {
        _path = path;
    }

    public T Read(int id)
    {
        var fileName = GetEntityUniqueName(id);
        return Resources.Load<T>(_path + fileName);
    }

    public IEnumerable<T> ReadAll() => Resources.LoadAll<T>(_path);

    public int Create(T entity) => throw new System.NotSupportedException("Create in resources not supported");

    public void Update(T entity) => throw new System.NotSupportedException("Update in resources not supported");

    public void Delete(T entity) => throw new System.NotSupportedException("Delete in resources not supported");
}
}