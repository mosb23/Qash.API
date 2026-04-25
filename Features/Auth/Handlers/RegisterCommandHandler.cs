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

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ApiResponse<AuthResponseDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly IJwtTokenService _jwtTokenService;

    public RegisterCommandHandler(
        ApplicationDbContext context,
        IPasswordHasherService passwordHasherService,
        IJwtTokenService jwtTokenService)
    {
        _context = context;
        _passwordHasherService = passwordHasherService;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<ApiResponse<AuthResponseDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLower();
        var phone = request.PhoneNumber.Trim();

        var exists = await _context.Users.AnyAsync(
            x => x.Email == email || x.PhoneNumber == phone,
            cancellationToken);

        if (exists)
        {
            return ApiResponse<AuthResponseDto>.FailResponse(
                "Registration failed",
                ["Email or phone number already exists"]);
        }

        var user = new ApplicationUser
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = email,
            PhoneNumber = phone,
            PasswordHash = _passwordHasherService.HashPassword(request.Password)
        };

        var tokenResult = _jwtTokenService.GenerateTokens(user);

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = tokenResult.RefreshToken,
            ExpiresAt = tokenResult.RefreshTokenExpiresAt,
            ApplicationUserId = user.Id
        });

        await _context.Users.AddAsync(user, cancellationToken);
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
            "Registration completed successfully");
    }
}