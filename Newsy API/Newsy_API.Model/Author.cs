namespace Newsy_API.Model
{
    /// <summary>
    /// An User who publishes and edit articles
    /// </summary>
    public class Author : User
    {
        public string Biography { get; set; } = string.Empty;
        public string Tagline { get; set; } = string.Empty;

        public List<Article> Articles { get; private set; } = new List<Article>();

        public Author(string firstName, string lastName, string eMail) : base(firstName, lastName, eMail)
        {
        }
    }
}
