
using LedgerCore.Application.Interface;

namespace Ledger.Persistence.stubs
{
    public class StubBalanceProvider:IBalanceProvider
    {
        public Task<decimal> GetBalance(string accountNumber) => Task.FromResult(0m);
    }
}
