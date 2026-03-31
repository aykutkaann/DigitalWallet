using DigitalWallet.Application.Interfaces;
using DigitalWallet.Application.Settings;
using DigitalWallet.Domain.Entities;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace DigitalWallet.Infrastructure.Security
{
    public class JwtTokenGenerator :IJwtTokenGenerator
    {
        private readonly JwtSettings _settings;

        public JwtTokenGenerator(IOptions<JwtSettings> jwtOptions)
        {
            _settings = jwtOptions.Value;
        }

        public string GenerateToken(User user)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(ClaimTypes.Role, user.Role.ToString()), 
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) 
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_settings.ExpirationInMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
