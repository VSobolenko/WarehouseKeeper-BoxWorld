using WarehouseKeeper.Repositories;

namespace WarehouseKeeper.Managers.Repositories
{
public class BaseSaveManager<T> where T : IHasBasicId
{
    // ToDo: для всех реализаций добавить кэш
    protected virtual string GetEntityUniqueName(T entity) => typeof(T).ToString() + entity.Id;
    protected virtual string GetEntityUniqueName(int id) => typeof(T).ToString() + id;
}
}