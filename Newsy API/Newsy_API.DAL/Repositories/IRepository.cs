using Microsoft.EntityFrameworkCore.Storage;
using Newsy_API.Model;

namespace Newsy_API.DAL.Repositories
{
    public interface IRepository<T> where T : Entity
    {
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Finds entity by its Id
        /// </summary>
        /// <param name="id">Entity Id</param>
        /// <returns>Found entity</returns>
        /// <exception cref="typeof(NotFoundException)">Entity not found</exception>
        Task<T> GetByIdAsync(long id);
        Task InsertAsync(T entity);
        Task DeleteAsync(long id);

        /// <summary>
        /// Starts database transaction
        /// </summary>
        /// <returns>Transaction context</returns>
        /// <remarks>Should be used for complex operations where manual commit and rolleback are needed. 
        /// If operation is not commited or rollbacked, can cause problems with application flow, so use it carefully
        /// </remarks>
        IDbContextTransaction BeginTransaction();
    }
}
