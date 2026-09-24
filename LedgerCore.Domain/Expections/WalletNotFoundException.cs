using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedgerCore.Domain.Expections
{
    public class WalletNotFoundException : Exception
    {
        public WalletNotFoundException(string accountNumber)
            : base($"Wallet '{accountNumber}' was not found.") { }
    }

    public class DuplicateAccountNumberException : Exception
    {
        public DuplicateAccountNumberException(string accountNumber)
            : base($"Account number '{accountNumber}' already exists.") { }
    }
}
