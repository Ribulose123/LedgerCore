
using LedgerCore.Domain.Entities.Wallet;

namespace LedgerCore.Domain.IRepository
{
    public interface IWalletRepository
    {
        Task<Wallet?> GetByAccountNumber(string accountNumber);
        Task Add(Wallet wallet);
        Task SaveChangesAsync();

        void Detach(Wallet wallet);
    }
}
