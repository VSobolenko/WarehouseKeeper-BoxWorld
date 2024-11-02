using System.Linq;
using System.Threading.Tasks;

namespace Game.Repositories
{
internal interface IRepositoryAsync<T>
{
    Task<int> Create(T entity);

    Task<T> Read(int id);

    Task<IQueryable<T>> ReadAll();

    Task Update(T entity);

    Task Delete(T entity);
}
}