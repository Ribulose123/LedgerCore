using LedgerCore.Application.Dtos.AuthDto;
using LedgerCore.Application.Exceptions;
using LedgerCore.Application.Interface;
using LedgerCore.Application.Settings;
using LedgerCore.Domain.Entities.Token;
using LedgerCore.Domain.Enum;
using LedgerCore.Domain.IRepository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LedgerCore.Application.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly ITokenHasher _tokenHasher;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly RefreshTokenSettings _refreshTokenSettings;
        private readonly ILogger<AuthService> _logger;



        public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator, ITokenGenerator tokenGenerator, ITokenHasher tokenHasher, IRefreshTokenRepository refreshTokenRepository, IOptions<RefreshTokenSettings> refreshOption, ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
            _tokenGenerator = tokenGenerator;
            _tokenHasher = tokenHasher;
            _refreshTokenRepository = refreshTokenRepository;
            _refreshTokenSettings = refreshOption.Value;
            _logger = logger;
        }

        public async Task<AuthResult> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetUserByEmail(email);
            
            if(user is null)
            {
                throw new InvalidCredentialsException();
            }

            if(user.Status != Domain.Enum.UserStatus.Active)
            {
                throw new InvalidCredentialsException();
            }

            var outcome = _passwordHasher.Verify(password, user.PasswordHash);

            if(outcome == Enum.PasswordVerificationOutcome.Failed)
            {
                throw new InvalidCredentialsException();
            }

            var roleId = user.GetCurrentRoleId();

            var accessToken = _jwtTokenGenerator.GenerateAccessToken(user.Id, roleId);

            var rawRefreshToken = _tokenGenerator.GenerateRawToken();

            var refreshTokenHash = _tokenHasher.Hash(rawRefreshToken);

            var refreshToken = new RefreshToken(user.Id, refreshTokenHash,TimeSpan.FromDays(_refreshTokenSettings.ExpirationDays));

            await _refreshTokenRepository.AddAsync(refreshToken);
            await _refreshTokenRepository.SaveChangesAsync();

            return new AuthResult(accessToken, rawRefreshToken, user.Name, user.Email);

        }

        public async Task<AuthResult> RefreshAsync(
      string rawRefreshToken)
        {
            var tokenHash = _tokenHasher.Hash(rawRefreshToken);

            var refreshToken =
                await _refreshTokenRepository.GetByTokenHashAsync(tokenHash);

            if (refreshToken is null)
            {
                throw new InvalidRefreshTokenException();
            }

            
            
            if (refreshToken.ReplacedByTokenId.HasValue)
            {
                _logger.LogWarning("Refresh token replay detected for user {UserId}.", refreshToken.UserId);

                throw new InvalidRefreshTokenException();
            }

            if (refreshToken.IsExpired)
            {
                throw new InvalidRefreshTokenException();
            }

            if (refreshToken.RevokedAt.HasValue)
            {
                throw new InvalidRefreshTokenException();
            }

            var user =
                await _userRepository.GetUserById(
                    refreshToken.UserId);

            if (user is null)
            {
                throw new InvalidRefreshTokenException();
            }

            if (user.Status != UserStatus.Active)
            {
                throw new InvalidRefreshTokenException();
            }

            var roleId = user.GetCurrentRoleId();

            var accessToken =
                _jwtTokenGenerator.GenerateAccessToken(
                    user.Id,
                    roleId);

            var newRawRefreshToken =
                _tokenGenerator.GenerateRawToken();

            var newRefreshTokenHash =
                _tokenHasher.Hash(newRawRefreshToken);

            var newRefreshToken = new RefreshToken(
                user.Id,
                newRefreshTokenHash,
                TimeSpan.FromDays(
                    _refreshTokenSettings.ExpirationDays));

            refreshToken.ReplaceToken(
                newRefreshToken.Id);

            await _refreshTokenRepository.AddAsync(
                newRefreshToken);

            await _refreshTokenRepository.SaveChangesAsync();

            return new AuthResult(
                accessToken,
                newRawRefreshToken, user.Name, user.Email);
        }


        public async Task LogoutAsync(string rawRefreshToken)
        {
            var tokenHash = _tokenHasher.Hash(rawRefreshToken);

            var refreshToken = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash);

            if(refreshToken is null)
            {
                throw new InvalidRefreshTokenException();
            }
            if(refreshToken.RevokedAt.HasValue)
            {
                throw new InvalidRefreshTokenException();
            }

            if(refreshToken.IsExpired)
            {
                throw new InvalidRefreshTokenException();
            }

            refreshToken.Revoke();

            await _refreshTokenRepository.SaveChangesAsync();
        }
    }
}
