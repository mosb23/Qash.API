using MediatR;
using Microsoft.EntityFrameworkCore;
using Qash.API.Common.Responses;
using Qash.API.Domain.Entities;
using Qash.API.Features.Auth.Commands;
using Qash.API.Features.Auth.DTOs;
using Qash.API.Infrastructure.Authentication;
using Qash.API.Infrastructure.Data;
using Qash.API.Infrastructure.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Qash.API.Features.Auth.Handlers;

public class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResponse<AuthResponseDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginCommandHandler(
        ApplicationDbContext context,
        IPasswordHasherService passwordHasherService,
        IJwtTokenService jwtTokenService)
    {
        _context = context;
        _passwordHasherService = passwordHasherService;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<ApiResponse<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var phone = request.PhoneNumber.Trim();

        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.PhoneNumber == phone, cancellationToken);

        if (user is null)
        {
            return ApiResponse<AuthResponseDto>.FailResponse(
                "Login failed.",
                ["Invalid phone number or password."]);
        }

        var validPassword = _passwordHasherService.VerifyPassword(
            request.Password,
            user.PasswordHash);

        if (!validPassword)
        {
            return ApiResponse<AuthResponseDto>.FailResponse(
                "Login failed.",
                ["Invalid phone number or password."]);
        }

        if (!user.IsPhoneNumberVerified)
        {
            return ApiResponse<AuthResponseDto>.FailResponse(
                "Login failed.",
                ["Phone number is not verified. Please verify your phone number first."]);
        }

        await _context.RefreshTokens
            .Where(x =>
                x.ApplicationUserId == user.Id &&
                !x.IsRevoked &&
                !x.IsDeleted &&
                x.ExpiresAt > DateTime.UtcNow)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.IsRevoked, true)
                .SetProperty(x => x.RevokedAt, DateTime.UtcNow)
                .SetProperty(x => x.UpdatedAt, DateTime.UtcNow),
                cancellationToken);

        var tokenResult = _jwtTokenService.GenerateTokens(user);

        var refreshToken = new RefreshToken
        {
            Token = tokenResult.RefreshToken,
            ExpiresAt = tokenResult.RefreshTokenExpiresAt,
            ApplicationUserId = user.Id
        };

        await _context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<AuthResponseDto>.SuccessResponse(
            new AuthResponseDto
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                AccessToken = tokenResult.AccessToken,
                RefreshToken = tokenResult.RefreshToken
            },
            "Login completed successfully.");
    }
}