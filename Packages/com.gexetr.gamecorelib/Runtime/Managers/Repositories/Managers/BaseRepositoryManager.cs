namespace Game.Repositories.Managers
{
internal class BaseRepositoryManager<T> where T : IHasBasicId
{
    // ToDo: add cache fo all repository
    protected virtual string GetEntityUniqueName(T entity) => typeof(T).ToString() + entity.Id;
    protected virtual string GetEntityUniqueName(int id) => typeof(T).ToString() + id;
    protected virtual string FileRegexPattern() => @$"{typeof(T)}\d+.[A-Za-z]+$";
}
}