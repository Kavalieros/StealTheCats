using AutoMapper;
using StealTheCats.Dtos;
using StealTheCats.Models;

namespace StealTheCats.Helpers
{
    public class CatMappingProfile : Profile
    {
        public CatMappingProfile()
        {
            CreateMap<CatEntityFetchDto, CatEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CatId, opt => opt.MapFrom(src => src.Id!))
                .ForMember(dest => dest.Image, opt => opt.Ignore())
                .ForMember(dest => dest.Created, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(dest => dest.Tags, opt => opt.Ignore());
        }
    }
}
