using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedgerCore.Application.Dtos.WalletDtos
{
    public record WalletActionRequest(Guid ActingUserId, string? Reason);
}
