using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedgerCore.Domain.Interface
{
    public interface IBalanceProvider
    {
        Task <decimal> GetBalance(string accountNumber);
    }
}
