using System.ComponentModel.DataAnnotations;

namespace Newsy_API.Model
{
    /// <summary>
    /// News writen by one of the authors in newsy system
    /// </summary>
    public class Article : Entity
    {
        [Required]
        public string Title { get; protected set; } = string.Empty;
        public string Text { get; protected set; } = string.Empty;
        public DateTime Created { get; protected set; }
        public DateTime Edited { get; protected set; }

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
            Created = DateTime.UtcNow;
        }

        public void ChangeContent(string title, string text)
        {
            Title = title;
            Text = text;
            Edited = DateTime.UtcNow;
        }
    }
}
