using LedgerCore.Domain.Enum;
using System;

namespace LedgerCore.Domain.Entities.Wallet
{
    public class WalletStatusHistory
    {
        public Guid WalletStatusHistoryId { get; private set; }
        public string WalletId { get; private set; } = string.Empty;
        public WalletStatus FromStatus { get; private set; }
        public WalletStatus ToStatus { get; private set; }
        public Guid ChangedByWho { get; private set; }
        public string? Reason { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private WalletStatusHistory() { }

        public WalletStatusHistory(string walletId, WalletStatus fromStatus, WalletStatus toStatus,
            Guid changedByWho, string? reason)
        {
            WalletStatusHistoryId = Guid.NewGuid();
            WalletId = walletId;
            FromStatus = fromStatus;
            ToStatus = toStatus;
            ChangedByWho = changedByWho;
            Reason = reason;
            CreatedAt = DateTime.UtcNow;
        }
    }
}