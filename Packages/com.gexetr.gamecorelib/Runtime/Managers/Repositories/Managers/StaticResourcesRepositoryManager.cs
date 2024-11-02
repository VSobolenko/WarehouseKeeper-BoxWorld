using System.Collections.Generic;
using Game.IO;
using UnityEngine;

namespace Game.Repositories.Managers
{
internal class StaticResourcesRepositoryManager<T> : BaseRepositoryManager<T>, IRepository<T> where T : class, IHasBasicId
{
    private readonly string _path;
    private readonly ISaveFile _saveFile;

    public StaticResourcesRepositoryManager(string path, ISaveFile saveFile)
    {
        _path = path;
        _saveFile = saveFile;
        
        if (typeof(T).IsSubclassOf(typeof(Object)))
            throw new System.NotSupportedException("Unity object not supported in this repository");
    }

    public T Read(int id)
    {
        var textAsset = Resources.Load<TextAsset>(_path + GetEntityUniqueName(id));

        return textAsset == null ? null : _saveFile.Deserialize<T>(textAsset.bytes);
    }

    public IEnumerable<T> ReadAll()
    {
        var allTexts = Resources.LoadAll<TextAsset>(_path);
        var allFiles = new List<T>(allTexts.Length - 1);
        
        for (var i = 0; i < allTexts.Length; i++)
        {
            var entity = _saveFile.Deserialize<T>(allTexts[i].bytes);
            allFiles.Add(entity);
        }

        return allFiles;
    }

    public int Create(T entity) => throw new System.NotSupportedException("Create in resources not supported");

    public void CreateById(T entity) => throw new System.NotSupportedException("Create in resources not supported");

    public void Update(T entity) => throw new System.NotSupportedException("Update in resources not supported");

    public void Delete(T entity) => throw new System.NotSupportedException("Delete in resources not supported");
}
}