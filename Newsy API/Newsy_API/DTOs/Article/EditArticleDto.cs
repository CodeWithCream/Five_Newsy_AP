using System.ComponentModel.DataAnnotations;

namespace Newsy_API.DTOs.Article
{
    public class EditArticleDto
    {
        [Required(AllowEmptyStrings = false)]
        public string Title { get; set; } = null!;

        [Required(AllowEmptyStrings = false)]
        public string Text { get; set; } = null!;
    }
}
