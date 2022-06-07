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
        public long? AuthorId { get; protected set; }
        public Author? Author { get; protected set; }

        protected Article()
        {
        }

        public Article(string title, string text, long authorId) : base()
        {
            Title = title;
            AuthorId = authorId;
            Text = text;
            Created = DateTime.UtcNow;
            Edited = Created;
        }

        public void ChangeContent(string title, string text)
        {
            Title = title;
            Text = text;
            Edited = DateTime.UtcNow;
        }
    }
}
