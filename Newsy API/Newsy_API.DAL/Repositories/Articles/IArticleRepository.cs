using Newsy_API.Model;

namespace Newsy_API.DAL.Repositories.Articles
{
    public interface IArticleRepository : IRepository<Article>
    {
        Task<IEnumerable<Article>> GetAllAsync(int pageSize, int pageNumber);

        Task UpdateAsync(long id, string title, string text);

        Task<int> CountAsync();
    }
}
