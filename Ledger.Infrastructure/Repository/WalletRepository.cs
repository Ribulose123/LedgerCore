using Ledger.Persistence.Context;
using LedgerCore.Domain.Entities.Wallet;
using LedgerCore.Domain.Expections;
using LedgerCore.Domain.IRepository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Ledger.Persistence.Repository
{
   public class WalletRepository:IWalletRepository
    {
        private readonly AppDbContext _context;

        public WalletRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Wallet?> GetByAccountNumber(string accountNumber)
        {
            return await _context.Wallets.Include(w => w.StatusHistory)
                .FirstOrDefaultAsync(w => w.AccountNumber == accountNumber);
        }

       
      

        public async Task Add(Wallet wallet)
        {
            await _context.Wallets.AddAsync(wallet);
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
            {
                var pendingWallet = _context.ChangeTracker.Entries<Wallet>()
                    .FirstOrDefault(e => e.State == EntityState.Added)?.Entity;

                throw new DuplicateAccountNumberException(pendingWallet?.AccountNumber ?? "unknown");
            }
        }
        public void Detach(Wallet wallet)
        {
            _context.Entry(wallet).State = EntityState.Detached;
        }

        private static bool IsUniqueConstraintViolation(DbUpdateException ex) =>
            ex.InnerException is SqlException sqlEx &&
            (sqlEx.Number == 2601 || sqlEx.Number == 2627);
    }
}
