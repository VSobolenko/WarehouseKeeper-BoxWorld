using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Game.IO;

namespace Game.Repositories.Managers
{
internal class FileRepositoryManager<T> : BaseRepositoryManager<T>, IRepository<T> where T : class, IHasBasicId
{
    private readonly string _path;
    private readonly ISaveFile _saveFile;
    private readonly DirectoryInfo _directoryInfo;

    private Regex _filesRegex;
    
    // public FileRepositoryManager(string path, ISaveFile saveFile, bool deleteExistingFiles)
    // {
    //     if (string.IsNullOrEmpty(path))
    //         throw new ArgumentNullException(nameof(path), "Path cannot be null or empty");
    //     _path = path;
    //    _saveFile = saveFile;
    //
    //     PrepareDirectory(deleteExistingFiles);
    //     _directoryInfo = new DirectoryInfo(_path);
    // }
    
    public FileRepositoryManager(string path, ISaveFile saveFile)
    {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException(nameof(path), "Path cannot be null or empty");
        _path = path;
        _saveFile = saveFile;

        PrepareDirectory(true);
        _directoryInfo = new DirectoryInfo(_path);
    }

    public int Create(T entity)
    {
        var fileId = GetFreFileId();
        CreateFileById(fileId, entity);

        return fileId;
    }

    public void CreateById(T entity)
    {
        CreateFileById(entity.Id, entity);
    }

    public T Read(int id)
    {
        if (IsFileExists(id) == false)
            return null;
        
        var fileName = GetEntityUniqueName(id);
        return _saveFile.Read<T>(_path + fileName);
    }

    public IEnumerable<T> ReadAll()
    {
        var freeFileId = GetFreFileId();
        var allFiles = new List<T>(Math.Clamp(freeFileId, 0, int.MaxValue));
        
        for (var i = 0; i < freeFileId; i++)
        {
            if (IsFileExists(i) == false)
                continue;

            var fileName = GetEntityUniqueName(i);
            var entity = _saveFile.Read<T>(_path + fileName);
            allFiles.Add(entity);
        }

        return allFiles;
    }

    public void Update(T entity)
    {
        if (IsFileExists(entity) == false)
        {
            Log.Warning($"Cannot find file with id={entity.Id}. Update skipped");

            return;
        }
        
        Delete(entity);
        CreateFileById(entity.Id, entity);
    }

    public void Delete(T entity)
    {
        if (IsFileExists(entity) == false)
        {
            Log.Warning($"Cannot find file with id={entity.Id}. Delete skipped");

            return;
        }

        var fileName = GetEntityUniqueName(entity);
        _saveFile.Delete(_path + fileName);
    }

    private void CreateFileById(int id, T entity)
    {
        if (IsFileExists(id))
        {
            Log.Error($"File {typeof(T).ToString() + id} with id={id} already exist. File creation skipped!");
            return;
        }

        entity.Id = id;
        var fileName = GetEntityUniqueName(id);
        _saveFile.Write(_path + fileName, entity, FileMode.Create);
    }

    private int GetFreFileId()
    {
        if (_directoryInfo.GetFiles().Length <= 0)
            return 0;

        _filesRegex ??= new Regex(FileRegexPattern());

        var file = _directoryInfo.GetFiles()?.Where(x => _filesRegex.IsMatch(x.Name))?.OrderBy(x => x.Name.Length)
                                 ?.ThenBy(x => x.Name)?.LastOrDefault();
        var fileName = Path.GetFileNameWithoutExtension(file?.Name);
        if (string.IsNullOrEmpty(fileName))
        {
            Log.Error("Cannot find free file ID");

            return int.MaxValue;
        }

        var fileIdBias = fileName.Length - GetEntityUniqueName(0).Length;

        var lastFileId = new StringBuilder();
        for (var i = fileName.Length - 1 - fileIdBias; i < fileName.Length; i++)
            lastFileId.Append(fileName[i]);

        return Convert.ToInt32(lastFileId.ToString()) + 1;
    }

    private void PrepareDirectory(bool deleteExistingFiles)
    {
        if ((_path[^1] == '\\' || _path[^1] == '/') == false)
            Log.Warning($"Possibly specified path. Path: {_path}");

        if (Directory.Exists(_path) == false)
        {
            Directory.CreateDirectory(_path);
            Log.Info($"New repository path created. Path: {_path}");

            return;
        }

        if (deleteExistingFiles == false)
            return;

        var directoryInfo = new DirectoryInfo(_path);
        
        foreach (var file in directoryInfo.GetFiles())
        {
            try
            {
                if (file.Name.Contains(typeof(T).ToString()))
                    continue;
#if UNITY_EDITOR
                Log.Warning($"The file repository contains the file {file.Name}. Folder must be empty. " +
                                 $"This file will be deleted. File path: {_path}");
#else
                Log.Warning($"Delete file {file.Name}. Folder must be empty. Path: {_path}");
                file.Delete();
#endif
            }
            catch (Exception e)
            {
                Log.Error($"Cannot delete file in directory: {_path}. Exception: {e.Message}");
                
                throw new ArgumentException($"Directory cannot contains file: {file.Name} in {_path}");
            }
        }
    }

    private bool IsFileExists(T entity) => _saveFile.IsFileExist(_path + typeof(T) + entity.Id);
    private bool IsFileExists(int id) => _saveFile.IsFileExist(_path + typeof(T) + id);
    private bool IsFileExists(string fileName) => _saveFile.IsFileExist(_path + fileName);
}
}