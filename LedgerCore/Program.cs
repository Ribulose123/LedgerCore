using Ledger.Persistence.Context;
using Ledger.Persistence.Repository;
using Ledger.Persistence.Security;
using Ledger.Persistence.Service;
using Ledger.Persistence.stubs;
using LedgerCore.Application.Interface;
using LedgerCore.Application.Services;
using LedgerCore.Application.Settings;
using LedgerCore.Domain.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// --------------------
// Application services
// --------------------

builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<IBalanceProvider, StubBalanceProvider>();
builder.Services.AddScoped<IUserRoleQuery, StubUserRoleQuery>();
builder.Services.AddScoped<WalletService>();

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<UserService>();

builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddSingleton<ITokenGenerator, TokenGenerator>();
builder.Services.AddSingleton<ITokenHasher, TokenHasher>();

builder.Services.AddScoped<AuthService>();


// --------------------
// JWT configuration
// --------------------

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt"));

builder.Services.Configure<RefreshTokenSettings>(
    builder.Configuration.GetSection("RefreshToken"));

builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();


// --------------------
// Authentication
// --------------------

var jwtSettings = builder.Configuration
    .GetSection("Jwt");

var secretKey = jwtSettings["SecretKey"];

if (string.IsNullOrWhiteSpace(secretKey))
{
    throw new InvalidOperationException(
        "JWT secret key is not configured.");
}

var issuer = jwtSettings["Issuer"];

if (string.IsNullOrWhiteSpace(issuer))
{
    throw new InvalidOperationException(
        "JWT issuer is not configured.");
}

var audience = jwtSettings["Audience"];

if (string.IsNullOrWhiteSpace(audience))
{
    throw new InvalidOperationException(
        "JWT audience is not configured.");
}

builder.Services
    .AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = issuer,

                ValidateAudience = true,
                ValidAudience = audience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(secretKey)),

                ValidateLifetime = true,

                ClockSkew = TimeSpan.Zero
            };
    });


// --------------------
// Controllers
// --------------------

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter()));


// --------------------
// Database
// --------------------

builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlServer(
        builder.Configuration
            .GetConnectionString("DefaultConnection")));


// --------------------
// Swagger
// --------------------

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();


// --------------------
// HTTP request pipeline
// --------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();