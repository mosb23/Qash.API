using MediatR;
using Microsoft.EntityFrameworkCore;
using Qash.API.Common.Responses;
using Qash.API.Features.Auth.Commands;
using Qash.API.Infrastructure.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Qash.API.Features.Auth.Handlers;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, ApiResponse<string>>
{
    private readonly ApplicationDbContext _context;

    public LogoutCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<string>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var affectedRows = await _context.RefreshTokens
            .Where(x => x.Token == request.RefreshToken && !x.IsRevoked && !x.IsDeleted)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.IsRevoked, true)
                .SetProperty(x => x.RevokedAt, DateTime.UtcNow)
                .SetProperty(x => x.UpdatedAt, DateTime.UtcNow),
                cancellationToken);

        if (affectedRows == 0)
        {
            return ApiResponse<string>.FailResponse(
                "Logout failed.",
                ["Invalid refresh token."]);
        }

        return ApiResponse<string>.SuccessResponse(
            "Logged out",
            "Logout completed successfully.");
    }
}