using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WarehouseKeeper.Extension;
using WarehouseKeeper.Repositories;
using WarehouseKeeper.Repositories.FileRepositorySystem;

namespace WarehouseKeeper.Managers.Repositories
{
internal class FileSaveManager<T> : BaseSaveManager<T>, IRepository<T> where T : class, IHasBasicId
{
    private readonly string _path;
    private readonly IFileRepository _fileRepository;
    private readonly DirectoryInfo _directoryInfo;

    public FileSaveManager(string path, IFileRepository fileRepository)
    {
        _path = path;
       _fileRepository = fileRepository;

        PrepareDirectory();
        _directoryInfo = new DirectoryInfo(_path);
    }

    public int Create(T entity)
    {
        var fileId = GetFreFileId();
        CreateFileById(fileId, entity);

        return fileId;
    }

    public T Read(int id)
    {
        if (IsFileExists(id) == false)
            return null;
        
        var fileName = GetEntityUniqueName(id);
        return _fileRepository.Read<T>(_path + fileName);
    }

    public IEnumerable<T> ReadAll()
    {
        var freeFileId = GetFreFileId();
        var allFiles = new List<T>(freeFileId - 1);
        
        for (var i = 0; i < freeFileId; i++)
        {
            if (IsFileExists(i) == false)
                continue;

            var fileName = GetEntityUniqueName(i);
            var entity = _fileRepository.Read<T>(_path + fileName);
            allFiles.Add(entity);
        }

        return allFiles;
    }

    public void Update(T entity)
    {
        if (IsFileExists(entity) == false)
        {
            Log.WriteWarning($"Cannot find file with id={entity.Id}. Update skipped");

            return;
        }
        
        Delete(entity);
        CreateFileById(entity.Id, entity);
    }

    public void Delete(T entity)
    {
        if (IsFileExists(entity) == false)
        {
            Log.WriteWarning($"Cannot find file with id={entity.Id}. Delete skipped");

            return;
        }

        var fileName = GetEntityUniqueName(entity);
        _fileRepository.Delete(_path + fileName);
    }

    private void CreateFileById(int id, T entity)
    {
        if (IsFileExists(id))
        {
            Log.WriteError($"File {typeof(T).ToString() + id} with id={id} already exist. File creation skipped!");
            return;
        }
        
        var fileName = GetEntityUniqueName(id);
        _fileRepository.Write(_path + fileName, entity, FileMode.Create);
    }

    private int GetFreFileId()
    {
        if (_directoryInfo.GetFiles().Length <= 0)
            return 0;

        var lastSaveFile = _directoryInfo.GetFiles().OrderBy(x => x.Name).LastOrDefault();
        
        return lastSaveFile == null ? 0 : Convert.ToInt32(lastSaveFile.Name[^1]);
    }

    private void PrepareDirectory()
    {
        if (Directory.Exists(_path) == false)
        {
            Directory.CreateDirectory(_path);

            return;
        }

        var directoryInfo = new DirectoryInfo(_path);

        foreach (var file in directoryInfo.GetFiles())
        {
            try
            {
                if (file.Name.Contains(typeof(T).ToString()))
                    continue;
                
                Log.WriteWarning($"Delete file {file.Name}. Folder must be empty");
                file.Delete();
            }
            catch (Exception e)
            {
                Log.WriteError($"Cannot delete file in directory: {_path}. Exception: {e.Message}");
                
                throw new ArgumentException($"Directory cannot contains file: {file.Name} in {_path}");
            }
        }
    }

    private bool IsFileExists(T entity) => _fileRepository.IsFileExist(_path + typeof(T) + entity.Id);
    private bool IsFileExists(int id) => _fileRepository.IsFileExist(_path + typeof(T) + id);
    private bool IsFileExists(string fileName) => _fileRepository.IsFileExist(_path + fileName);
}
}