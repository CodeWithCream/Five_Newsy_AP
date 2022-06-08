using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Newsy_API.DTOs
{
    public class PaginationFilter
    {
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        [DefaultValue(1)]
        public int PageNumber { get; set; } = 1;

        [Range(0, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        [DefaultValue(10)]
        public int PageSize { get; set; } = 10;
    }
}
