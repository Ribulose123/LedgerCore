using LedgerCore.Application.Constants;
using LedgerCore.Application.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace LedgerCore.Security
{
    public sealed class ConfigureJwtBearerOptions : IConfigureNamedOptions<JwtBearerOptions>
    {
        private readonly JwtSettings _jwt;
        public ConfigureJwtBearerOptions(IOptions<JwtSettings> jwt) => _jwt = jwt.Value;

        public void Configure(string? name, JwtBearerOptions options)
        {
            if (name != "Bearer") return;

            options.MapInboundClaims = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _jwt.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwt.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SecretKey)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                NameClaimType = JwtClaimTypes.Subject
            };
        }

        public void Configure(JwtBearerOptions options) => Configure("Bearer", options);
    }
}
