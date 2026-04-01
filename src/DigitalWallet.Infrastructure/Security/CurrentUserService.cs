using DigitalWallet.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DigitalWallet.Infrastructure.Security
{
    // This service acts as a bridge between HTTP layer and Application layer.
    // It reads the JWT token claims from the current HTTP request
    // so that services (WalletService, etc.) can know WHO is making the request.
    public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
    {
        public Guid UserId
        {
            get
            {
                // JWT "sub" claim can be mapped to different claim types depending on config.
                // We check both to be safe:
                // 1. ClaimTypes.NameIdentifier — ASP.NET's default mapping for "sub"
                // 2. JwtRegisteredClaimNames.Sub — the raw JWT claim name ("sub")
                var userIdClaim = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
                               ?? httpContextAccessor.HttpContext?.User?.FindFirst(JwtRegisteredClaimNames.Sub);

                if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
                {
                    return userId;
                }

                throw new UnauthorizedAccessException("User ID not found in token.");
            }
        }

        // Checks if the current request has an authenticated user
        public bool IsAuthenticated =>
            httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}
