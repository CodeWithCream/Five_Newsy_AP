using AutoMapper;
using Newsy_API.DTOs;
using Newsy_API.Model;

namespace Newsy_API.Mappings
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Author, AuthorDto>();
        }
    }
}
