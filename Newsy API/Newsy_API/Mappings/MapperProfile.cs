using AutoMapper;
using Newsy_API.DTOs.Article;
using Newsy_API.DTOs.Author;
using Newsy_API.DTOs.User;
using Newsy_API.Model;

namespace Newsy_API.Mappings
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Author, AuthorBasicDto>();
            CreateMap<Author, AuthorDto>();

            CreateMap<Article, ArticleDto>();
            CreateMap<CreateArticleDto, Article>()
                .ConstructUsing(src => new Article(src.Title, src.Text, src.AuthorId));

            CreateMap<RegisterUserDto, User>()
                .ConstructUsing(src => new User(src.FirstName, src.LastName, src.EMail));
            CreateMap<RegisterUserDto, Author>()
                .ConstructUsing(src => new Author(src.FirstName, src.LastName, src.EMail));
        }
    }
}
