using Newsy_API.Model;

namespace Newsy_API.DAL.Repositories
{
    public interface IRepository<T> where T : Entity
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(long id);
        Task InsertAsync(T entity);
        Task DeleteAsync(long id);
    }
}
