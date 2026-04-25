using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Qash.API.Common.Responses;
using Qash.API.Features.Profile.DTOs;
using Qash.API.Features.Profile.Queries;
using Qash.API.Infrastructure.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Qash.API.Features.Profile.Handlers;

public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, ApiResponse<ProfileDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetProfileQueryHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<ProfileDto>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

        if (user is null)
        {
            return ApiResponse<ProfileDto>.FailResponse(
                "Profile not found.",
                ["User profile was not found."]);
        }

        var profile = _mapper.Map<ProfileDto>(user);

        return ApiResponse<ProfileDto>.SuccessResponse(
            profile,
            "Profile retrieved successfully.");
    }
}