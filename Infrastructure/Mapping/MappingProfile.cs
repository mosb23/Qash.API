using AutoMapper;
using Qash.API.Domain.Entities;
using Qash.API.Features.Auth.DTOs;
using Qash.API.Features.Profile.DTOs;
using Qash.API.Features.Transactions.DTOs;
using Qash.API.Features.Wallet.DTOs;

using TransactionEntity = Qash.API.Domain.Entities.Transaction;
using TransactionDtoModel = Qash.API.Features.Transactions.DTOs.TransactionDto;

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

        CreateMap<Wallet, WalletDto>()
            .ForMember(dest => dest.WalletId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.ApplicationUserId));

        CreateMap<TransactionEntity, TransactionDtoModel>()
            .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.WalletId, opt => opt.MapFrom(src => src.WalletId))
            .ForMember(dest => dest.WalletName, opt => opt.MapFrom(src => src.Wallet.Name))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.ApplicationUserId));
    }
}