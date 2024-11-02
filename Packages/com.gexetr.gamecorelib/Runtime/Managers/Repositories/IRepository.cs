using System.Collections.Generic;

namespace Game.Repositories
{
public interface IRepository<T> where T : class, IHasBasicId
{
    /// <summary>
    /// Create new entity with new unique ID
    /// </summary>
    /// <param name="entity"></param>
    /// <returns>entity ID</returns>
    int Create(T entity);
    
    /// <summary>
    /// Create new entity by their ID
    /// </summary>
    /// <param name="entity"></param>
    void CreateById(T entity);

    T Read(int id);

    IEnumerable<T> ReadAll();

    void Update(T entity);

    void Delete(T entity);
}
}