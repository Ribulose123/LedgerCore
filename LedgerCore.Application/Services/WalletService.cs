using LedgerCore.Application.Interface;
using LedgerCore.Domain.Entities.Wallet;
using LedgerCore.Domain.Expections;
using LedgerCore.Domain.IRepository;

namespace LedgerCore.Application.Services
{
    public class WalletService
    {
        private readonly IBalanceProvider _balanceProvider;
        private readonly IWalletRepository _walletRepository;
        private readonly IUserRoleQuery _userRoleQuery;

        public WalletService(
            IBalanceProvider balanceProvider,
            IWalletRepository walletRepository,
            IUserRoleQuery userRoleQuery)
        {
            _balanceProvider = balanceProvider;
            _walletRepository = walletRepository;
            _userRoleQuery = userRoleQuery;
        }

        public async Task<Wallet?> GetByAccountNumber(string accountNumber) =>
            await _walletRepository.GetByAccountNumber(accountNumber);

        public async Task<Wallet> CreateWallet(Guid userId, string currency)
        {
            const int maxAttempts = 5;

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                var accountNumber = GenerateUserAccountNumber();
                var wallet = new Wallet(userId, currency, accountNumber);

                await _walletRepository.Add(wallet);

                try
                {
                    await _walletRepository.SaveChangesAsync();
                    return wallet;
                }
                catch (DuplicateAccountNumberException)
                {
                    _walletRepository.Detach(wallet);
                }
            }

            throw new InvalidOperationException(
                $"Failed to generate a unique account number after {maxAttempts} attempts.");
        }

        public Task DisableWallet(string accountNumber, Guid actingUserId, string? reason) =>
            ExecuteWalletActionAsync(accountNumber, actingUserId,
                (wallet, actor) => wallet.Disable(actor, reason));

        public Task EnableWallet(string accountNumber, Guid actingUserId, string? reason) =>
            ExecuteWalletActionAsync(accountNumber, actingUserId,
                (wallet, actor) => wallet.Enable(actor, reason));

        public Task RequestClosure(string accountNumber, Guid actingUserId, string? reason) =>
            ExecuteWalletActionAsync(accountNumber, actingUserId,
                (wallet, actor) => wallet.RequestClosure(actor, reason));

        public Task RejectClosure(string accountNumber, Guid actingUserId, string? reason) =>
            ExecuteWalletActionAsync(accountNumber, actingUserId,
                (wallet, actor) => wallet.RejectClosure(actor, reason));

        public async Task ApproveClosure(string accountNumber, Guid actingUserId, string? reason)
        {
            var balance = await _balanceProvider.GetBalance(accountNumber);

            await ExecuteWalletActionAsync(accountNumber, actingUserId,
                (wallet, actor) => wallet.ApproveClosure(actor, balance, reason));
        }

        public async Task CloseDirect(string accountNumber, Guid actingUserId, string? reason)
        {
            var balance = await _balanceProvider.GetBalance(accountNumber);

            await ExecuteWalletActionAsync(accountNumber, actingUserId,
                (wallet, actor) => wallet.CloseDirect(actor, balance, reason));
        }

        // Helpers

        private async Task<Wallet> GetWalletOrThrow(string accountNumber)
        {
            return await _walletRepository.GetByAccountNumber(accountNumber)
                ?? throw new WalletNotFoundException(accountNumber);
        }

        private async Task ExecuteWalletActionAsync(
            string accountNumber, Guid actingUserId, Action<Wallet, WalletActor> action)
        {
            var wallet = await GetWalletOrThrow(accountNumber);
            var role = await _userRoleQuery.GetActorRole(actingUserId);
            var actor = new WalletActor(actingUserId, role);

            action(wallet, actor);

            await _walletRepository.SaveChangesAsync();
        }

        private static string GenerateUserAccountNumber() =>
            Random.Shared.Next(0, 1_000_000_000).ToString("D10");
    }
}