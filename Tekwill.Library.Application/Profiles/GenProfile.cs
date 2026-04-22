using AutoMapper;
using Tekwill.Library.Application.DTOs.Gens;
using Tekwill.Library.Domain.Entities;

namespace Tekwill.Library.Application.Profiles
{
    public class GenProfile : Profile
    {
        public GenProfile()
        {
            CreateMap<Gen, GenDto>().ReverseMap();
            CreateMap<CreateGenDto, Gen>().ReverseMap();
        }
    }
}
