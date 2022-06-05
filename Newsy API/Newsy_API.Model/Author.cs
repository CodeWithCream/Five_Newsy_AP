namespace Newsy_API.Model
{
    /// <summary>
    /// An User who publishes and edit articles
    /// </summary>
    public class Author : User
    {
        public string Biography { get; set; } = string.Empty;
        public string Tagline { get; set; } = string.Empty;

        //1-N
        public IList<Article> Articles { get; private set; } = new List<Article>();

        protected Author() : base() { }

        public Author(string firstName, string lastName, string eMail) : base(firstName, lastName, eMail)
        {
        }
    }
}
