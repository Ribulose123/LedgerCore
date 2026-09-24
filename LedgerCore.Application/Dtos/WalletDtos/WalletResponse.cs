using LedgerCore.Domain.Entities.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LedgerCore.Application.Dtos.WalletDtos
{
    public record WalletResponse(
         string AccountNumber,
         Guid UserId,
         string Currency,
         string Status,
         DateTime CreatedAt,
         DateTime LastUpdated)
    {
        public static WalletResponse FromDomain(Wallet wallet) =>
            new(wallet.AccountNumber, wallet.UserId, wallet.Currency,
                wallet.Status.ToString(), wallet.CreatedAt, wallet.LastUpdated);
    }


}
