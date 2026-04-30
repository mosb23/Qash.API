using Qash.API.Domain.Entities;

namespace Qash.API.Infrastructure.Authentication;

public interface IJwtTokenService
{
    TokenResult GenerateTokens(ApplicationUser user);
}