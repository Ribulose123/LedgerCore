using LedgerCore.Domain.Entities.Wallet;
using LedgerCore.Domain.Identity;
using LedgerCore.Domain.Entities.Token;
using Microsoft.EntityFrameworkCore;

namespace Ledger.Persistence.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<Wallet> Wallets => Set<Wallet>();
        public DbSet<WalletStatusHistory> WalletStatusHistories => Set<WalletStatusHistory>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}