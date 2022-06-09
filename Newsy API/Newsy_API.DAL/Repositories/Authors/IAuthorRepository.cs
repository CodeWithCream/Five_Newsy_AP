using Newsy_API.Model;

namespace Newsy_API.DAL.Repositories.Authors
{
    public interface IAuthorRepository : IRepository<Author>
    {
        Task<Author> GetByIdAsync(long id, bool loadArticles);
    }
}
