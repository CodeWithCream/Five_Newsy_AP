using Newsy_API.Model;

namespace Newsy_API.DAL.Repositories
{
    public interface IAuthorRepository : IRepository<Author>
    {
        Task<Author> GetByIdAsync(long id, bool loadArticles);
    }
}
