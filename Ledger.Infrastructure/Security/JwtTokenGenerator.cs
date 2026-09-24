using LedgerCore.Application.Interface;
using LedgerCore.Application.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ledger.Persistence.Security
{
    public class JwtTokenGenerator:IJwtTokenGenerator
    {
        private readonly JwtSettings _jwtSettings;

        public JwtTokenGenerator(IOptions<JwtSettings> options)
        {
            _jwtSettings = options.Value;
        }

        public string GenerateAccessToken(
           Guid userId,
           Guid roleId)
        {
            if(string.IsNullOrWhiteSpace(_jwtSettings.SecretKey))
            {
                throw new InvalidOperationException("Secret key is not configured.");
            }

            if(string.IsNullOrWhiteSpace(_jwtSettings.Issuer))
            {
                throw new InvalidOperationException("Issuer is not configured.");
            }

            if (string.IsNullOrWhiteSpace(_jwtSettings.Audience))
            {
                throw new InvalidOperationException(
                    "JWT audience is not configured.");
            }

            if(_jwtSettings.AccessTokenExpirationMinutes <= 0)
            {
                throw new InvalidOperationException("Access token expiration is not configured.");
            }

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub,
                userId.ToString()),

                new(JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString()),

                new("role_id",
                roleId.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                signingCredentials:credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);


        }
    }
}
