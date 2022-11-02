using System.Collections.Generic;

namespace WarehouseKeeper.Repositories
{
internal interface IRepository<T> where T : IHasBasicId
{
    int Create(T entity);

    T Read(int id);

    IEnumerable<T> ReadAll();

    void Update(T entity);

    void Delete(T entity);
}
}