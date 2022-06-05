using System.ComponentModel.DataAnnotations;

namespace Newsy_API.Model
{
    /// <summary>
    /// A person who uses an application 
    /// </summary>
    public class User : Entity
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string EMail { get; set; } = string.Empty;

        // potentital app upgrade - user favorite articles, reading history, comments, reactions
        // public IList<UserArticle> Favorites { get; private set; } = new List<UserArticle>(); - N-N
        // public IList<UserArticle> ReadArticles { get; private set; } = new List<UserArticle>(); - N-N
        // public IList<UserArticleComment> Comments {get; private set;} = new List<UserArticleComment>(); 1-1-N

        protected User() { }

        public User(string firstName, string lastName, string eMail)
        {
            FirstName = firstName;
            LastName = lastName;
            EMail = eMail;
        }
    }
}