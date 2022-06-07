using Newsy_API.DTOs.Author;

namespace Newsy_API.DTOs.Article
{
    public class ArticleDto
    {
        public string Title { get; protected set; } = string.Empty;
        public string Text { get; protected set; } = string.Empty;
        public DateTime Created { get; protected set; }
        public DateTime Edited { get; protected set; }
        public AuthorBasicDto Author { get; private set; } = null!;
    }
}
