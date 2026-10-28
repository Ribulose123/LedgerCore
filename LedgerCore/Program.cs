using Ledger.Persistence.Context;
using Ledger.Persistence.Repository;
using Ledger.Persistence.Security;
using Ledger.Persistence.Service;
using Ledger.Persistence.stubs;
using LedgerCore.Api.Security;
using LedgerCore.Application.Interface;
using LedgerCore.Application.Services;
using LedgerCore.Application.Settings;
using LedgerCore.Authorization;
using LedgerCore.Domain.IRepository;
using LedgerCore.Middleware;
using LedgerCore.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// --------------------
// Application services
// --------------------
builder.Services.AddHttpContextAccessor();

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
builder.Services.AddSingleton<IAuthorizationHandler, AdminRoleHandler>();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();


builder.Services
    .AddOptions<JwtSettings>()
    .Bind(builder.Configuration.GetSection("Jwt"))
    .Validate(
        s => !string.IsNullOrWhiteSpace(s.Issuer),
        "JWT issuer is not configured.")
    .Validate(
        s => !string.IsNullOrWhiteSpace(s.Audience),
        "JWT audience is not configured.")
    .Validate(
        s => !string.IsNullOrWhiteSpace(s.SecretKey) &&
             Encoding.UTF8.GetByteCount(s.SecretKey) >= 32,
        "JWT secret key must be at least 32 bytes.")
    .ValidateOnStart();

builder.Services.Configure<RefreshTokenSettings>(
    builder.Configuration.GetSection("RefreshToken"));

builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();


builder.Services
    .AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();

builder.Services
    .AddAuthentication("Bearer")
    .AddJwtBearer("Bearer");

// --------------------
// Authorization
// --------------------
builder.Services.AddAuthorization(options =>
{
   
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    options.AddPolicy("AdminOnly", policy =>
    {
        policy.Requirements.Add(
            new AdminRoleRequirement());
    });
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

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter your JWT access token."
        });

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
});

// --------------------
// Build application
// --------------------
var app = builder.Build();

// --------------------
// HTTP request pipeline
// --------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();