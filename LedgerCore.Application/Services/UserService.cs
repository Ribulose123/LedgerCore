using LedgerCore.Application.Interface;
using LedgerCore.Domain.Enum;
using LedgerCore.Domain.Expections;
using LedgerCore.Domain.Identity;
using LedgerCore.Domain.IRepository;
namespace LedgerCore.Application.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<User> CreateUser(string name, string email, string plainPassword)
        {
            if (await _userRepository.GetUserByEmail(email) is not null)
            {
                throw new DuplicateEmailException(email);
            }

            var hash = _passwordHasher.Hash(plainPassword);

            var user = new User(name, email, hash, RoleIds.User);

            await _userRepository.AddUser(user);
            await _userRepository.SaveChangeAsync();

            return user;
        }

      
    }
}
