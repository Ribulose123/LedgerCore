

using LedgerCore.Domain.Identity;

namespace LedgerCore.Domain.IRepository
{
    public interface IUserRepository
    {
        Task<User?> GetUserById(Guid id);
        Task<User?> GetUserByEmail(string email);
        Task AddUser(User user);
        Task SaveChangeAsync();
        void Detach(User user);

    }
}
