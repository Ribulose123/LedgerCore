using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedgerCore.Application.Enum
{
    public enum PasswordVerificationOutcome
    {
        Failed,
        Success,
        SuccessRehashNeeded
    }
}
