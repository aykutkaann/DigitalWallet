using DigitalWallet.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DigitalWallet.Infrastructure.Security
{
    public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
    {
        public Guid UserId
        {
            get
            {
                var userIdClaim = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
                               ?? httpContextAccessor.HttpContext?.User?.FindFirst(JwtRegisteredClaimNames.Sub);

                if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
                {
                    return userId;
                }

                throw new UnauthorizedAccessException("User ID not found in token.");
            }
        }

        public bool IsAuthenticated =>
            httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}
