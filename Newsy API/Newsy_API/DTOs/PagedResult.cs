using System.Collections;

namespace Newsy_API.DTOs
{
    public class PagedResult<T> where T : IEnumerable
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }

        public T Data { get; set; }

        public PagedResult(T data, int pageNumber, int pageSize, int totalPages, int totalRecords)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = totalPages;
            TotalRecords = totalRecords;
            Data = data;
        }
    }
}
