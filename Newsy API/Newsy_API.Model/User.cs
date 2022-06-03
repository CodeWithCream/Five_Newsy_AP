namespace Newsy_API.Model
{
    /// <summary>
    /// A person who uses an application 
    /// </summary>
    public class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EMail { get; set; }

        // potentital app upgrade - user favorite articles, reading history, comments, reactions
        // public List<UserArticle> Favorites { get; set; } = new List<UserArticle>(); - N-N
        // public List<UserArticle> ReadArticles { get; set; } = new List<UserArticle>(); - N-N
        // public List<UserArticleComment> Comments {get;set;} = new List<UserArticleComment>(); 1-1-N

        public User(string firstName, string lastName, string eMail)
        {
            FirstName = firstName;
            LastName = lastName;
            EMail = eMail;
        }
    }
}