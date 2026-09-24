using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedgerCore.Domain.Enum
{
    public enum WalletStatus
    {
        Active, 
        Disabled, 
        PendingClosure,
        Closed
    }
}
