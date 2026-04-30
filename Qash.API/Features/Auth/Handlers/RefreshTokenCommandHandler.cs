using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Qash.API.Common.Responses;
using Qash.API.Domain.Entities;
using Qash.API.Features.Auth.Commands;
using Qash.API.Features.Auth.DTOs;
using Qash.API.Infrastructure.Authentication;
using Qash.API.Infrastructure.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Qash.API.Features.Auth.Handlers;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ApiResponse<AuthResponseDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IMapper _mapper;

    public RefreshTokenCommandHandler(
        ApplicationDbContext context,
        IJwtTokenService jwtTokenService,
        IMapper mapper)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
        _mapper = mapper;
    }

    public async Task<ApiResponse<AuthResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var existingToken = await _context.RefreshTokens
            .AsNoTracking()
            .Include(x => x.ApplicationUser)
            .FirstOrDefaultAsync(x => x.Token == request.RefreshToken, cancellationToken);

        if (existingToken is null || !existingToken.IsActive)
        {
            return ApiResponse<AuthResponseDto>.FailResponse(
                "Refresh token failed.",
                ["Invalid or expired refresh token."]);
        }

        var user = existingToken.ApplicationUser;

        await _context.RefreshTokens
            .Where(x => x.Id == existingToken.Id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.IsRevoked, true)
                .SetProperty(x => x.RevokedAt, DateTime.UtcNow)
                .SetProperty(x => x.UpdatedAt, DateTime.UtcNow),
                cancellationToken);

        var tokenResult = _jwtTokenService.GenerateTokens(user);

        var newRefreshToken = new RefreshToken
        {
            Token = tokenResult.RefreshToken,
            ExpiresAt = tokenResult.RefreshTokenExpiresAt,
            ApplicationUserId = user.Id
        };

        await _context.RefreshTokens.AddAsync(newRefreshToken, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var response = _mapper.Map<AuthResponseDto>(user);
        response.AccessToken = tokenResult.AccessToken;
        response.RefreshToken = tokenResult.RefreshToken;

        return ApiResponse<AuthResponseDto>.SuccessResponse(
            response,
            "Token refreshed successfully.");
    }
}