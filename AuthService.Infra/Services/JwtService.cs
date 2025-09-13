using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using AuthService.Infra.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Infra.Services
{
    public class JwtService(IOptions<JwtOptions> options) : IJwtService
    {

        public string GenerateToken(IUser user)
        {
            var secret = options.Value.Secret
            ?? throw new InvalidOperationException("JWT Secret não configurado!");

            if (secret.Length < 16)
                throw new InvalidOperationException("JWT Secret precisa ter pelo menos 16 caracteres!");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.Email, user.Email!),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: options.Value.Issuer,
                audience: options.Value.Audience,
                claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token).ToString();
        }
    }
}
