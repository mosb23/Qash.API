using AutoMapper;
using Qash.API.Domain.Entities;
using Qash.API.Features.Auth.DTOs;
using Qash.API.Features.Profile.DTOs;

namespace Qash.API.Infrastructure.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ApplicationUser, AuthResponseDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName));

        CreateMap<ApplicationUser, ProfileDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName));
    }
}