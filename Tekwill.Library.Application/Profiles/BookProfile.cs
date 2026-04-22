using AutoMapper;
using Tekwill.Library.Application.DTOs.Books;
using Tekwill.Library.Domain.Entities;

namespace Tekwill.Library.Application.Profiles
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<Book, BookDto>().ReverseMap();
        }
    }
}
