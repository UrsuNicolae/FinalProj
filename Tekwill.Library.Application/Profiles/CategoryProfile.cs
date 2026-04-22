using AutoMapper;
using Tekwill.Library.Application.DTOs.Categories;
using Tekwill.Library.Domain.Entities;

namespace Tekwill.Library.Application.Profiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<CreateCategoryDto, Category>().ReverseMap();
        }
    }
}
