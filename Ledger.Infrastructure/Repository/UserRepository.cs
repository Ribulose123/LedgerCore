

using Ledger.Persistence.Context;
using LedgerCore.Domain.IRepository;
using LedgerCore.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Data.SqlClient;
using LedgerCore.Domain.Expections;

namespace Ledger.Persistence.Repository
{
    public class UserRepository:IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserById(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _context.Users.Include(e=> e.Roles).FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task AddUser(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task SaveChangeAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }catch(DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
            {
                var pendingUser = _context.ChangeTracker.Entries<User>()
                    .FirstOrDefault(e => e.State == EntityState.Added)?.Entity;

                throw new DuplicateEmailException(pendingUser?.Email ?? "unknown");
            }
        }

        public void Detach(User user)
        {
            _context.Entry(user).State = EntityState.Detached;
        }

        private static bool IsUniqueConstraintViolation(DbUpdateException ex) =>
                 ex.InnerException is SqlException sqlEx &&
                    (sqlEx.Number == 2601 || sqlEx.Number == 2627);
        
    }
}
