using System.ComponentModel.DataAnnotations;

namespace Newsy_API.Model
{
    /// <summary>
    /// News writen by one of the authors in newsy system
    /// </summary>
    public class Article : Entity
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;

        [Required]
        public long? AuthorId { get; set; }
        public Author? Author { get; protected set; }

        protected Article()
        {
        }

        public Article(string title, Author author) : base()
        {
            Title = title;
            Author = author;
        }
    }
}
