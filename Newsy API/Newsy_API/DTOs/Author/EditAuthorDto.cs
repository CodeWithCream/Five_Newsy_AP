using System.ComponentModel.DataAnnotations;

namespace Newsy_API.DTOs.Author
{
    public class EditAuthorDto
    {
        [Required(AllowEmptyStrings = false)]
        public string Biography { get; set; } = null!;

        [Required(AllowEmptyStrings = false)]
        public string Tagline { get; set; } = null!;
    }
}
