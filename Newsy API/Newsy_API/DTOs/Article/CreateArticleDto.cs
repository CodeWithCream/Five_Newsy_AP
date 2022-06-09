using System.ComponentModel.DataAnnotations;

namespace Newsy_API.DTOs.Article
{
    public class CreateArticleDto
    {
        [Required(AllowEmptyStrings = false)]
        public string Title { get; set; } = null!;

        [Required(AllowEmptyStrings = false)]
        public string Text { get; set; } = null!;

        public long AuthorId { get; private set; }

        public void SetAuthor(long authorId)
        {
            AuthorId = authorId;
        }
    }
}
