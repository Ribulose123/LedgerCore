using LedgerCore.Application.Dtos.AuthDto;
using LedgerCore.Application.Exceptions;
using LedgerCore.Application.Interface;
using LedgerCore.Application.Settings;
using LedgerCore.Domain.Entities.Token;
using LedgerCore.Domain.IRepository;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

       

        public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator, ITokenGenerator tokenGenerator, ITokenHasher tokenHasher, IRefreshTokenRepository refreshTokenRepository, IOptions<RefreshTokenSettings> refreshOption)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
            _tokenGenerator = tokenGenerator;
            _tokenHasher = tokenHasher;
            _refreshTokenRepository = refreshTokenRepository;
            _refreshTokenSettings = refreshOption.Value;
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

            return new AuthResult(accessToken, rawRefreshToken);

        }
    }
}
