using System.Collections.Generic;
using UnityEngine;
using WarehouseKeeper.Repositories;
using WarehouseKeeper.Repositories.FileRepositorySystem;

namespace WarehouseKeeper.Managers.Repositories
{
internal class StaticResourcesManager<T> : BaseSaveManager<T>, IRepository<T> where T : IHasBasicId
{
    private readonly string _path;
    private readonly IFileRepository _fileRepository;

    public StaticResourcesManager(string path, IFileRepository fileRepository)
    {
        _path = path;
        _fileRepository = fileRepository;
        
        if (typeof(T).IsSubclassOf(typeof(Object)))
            throw new System.NotSupportedException("Unity object not supported in this repository");
    }

    public T Read(int id)
    {
        var textAsset = Resources.Load<TextAsset>(_path + GetEntityUniqueName(id));

        return _fileRepository.Deserialize<T>(textAsset.bytes);
    }

    public IEnumerable<T> ReadAll()
    {
        var allTexts = Resources.LoadAll<TextAsset>(_path);
        var allFiles = new List<T>(allTexts.Length - 1);
        
        for (var i = 0; i < allTexts.Length; i++)
        {
            var entity = _fileRepository.Deserialize<T>(allTexts[i].bytes);
            allFiles.Add(entity);
        }

        return allFiles;
    }

    public int Create(T entity) => throw new System.NotSupportedException("Create in resources not supported");

    public void Update(T entity) => throw new System.NotSupportedException("Update in resources not supported");

    public void Delete(T entity) => throw new System.NotSupportedException("Delete in resources not supported");
}
}