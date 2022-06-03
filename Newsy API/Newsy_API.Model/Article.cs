namespace Newsy_API.Model
{
    /// <summary>
    /// News writen by one of the authors in newsy system
    /// </summary>
    public class Article
    {
        public string Title { get; set; }
        public string Text { get; set; } = string.Empty;

        public Article(string title)
        {
            Title = title;
        }
    }
}
