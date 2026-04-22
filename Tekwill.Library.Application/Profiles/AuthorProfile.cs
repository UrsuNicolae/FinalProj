using AutoMapper;
using Tekwill.Library.Application.DTOs.Authors;
using Tekwill.Library.Domain.Entities;

namespace Tekwill.Library.Application.Profiles
{
    public class AuthorProfile : Profile
    {
        public AuthorProfile()
        {
            CreateMap<Author, AuthorDto>().ReverseMap();
            CreateMap<CreateAuthorDto, Author>().ReverseMap();
            CreateMap<UpdateAuthorDto, Author>()
                .ForMember(m => m.Books, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
