using LedgerCore.Domain.Enum;
using System;
using System.Collections.Generic;

namespace LedgerCore.Domain.Entities.Wallet
{
    public class Wallet
    {
        public string AccountNumber { get; private set; } = string.Empty;
        public Guid UserId { get; private set; }
        public string Currency { get; private set; } = string.Empty;
        public WalletStatus Status { get; private set; }
        public Guid? DisabledByUserId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime LastUpdated { get; private set; }

        private readonly List<WalletStatusHistory> _statusHistory = new();
        public IReadOnlyCollection<WalletStatusHistory> StatusHistory => _statusHistory.AsReadOnly();

        private Wallet() { }

        public Wallet(Guid user, string currency, string accountNumber)
        {
            if (user == Guid.Empty)
                throw new ArgumentException("User cannot be empty", nameof(user));

            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency is required", nameof(currency));

            if (string.IsNullOrWhiteSpace(accountNumber))
                throw new ArgumentException("Account number is required", nameof(accountNumber));

            AccountNumber = accountNumber;
            UserId = user;
            Currency = currency;
            Status = WalletStatus.Active;
            CreatedAt = DateTime.UtcNow;
            LastUpdated = DateTime.UtcNow;
        }

        public void Disable(WalletActor actor, string? reason = null)
        {
            EnsureActive();
            EnsureAuthorization(actor, allowSelf: true, allowAdmin: true);

            var from = Status;
            Status = WalletStatus.Disabled;
            DisabledByUserId = actor.UserId;
            Touch();

            RecordHistory(from, Status, actor.UserId, reason);
        }

        public void Enable(WalletActor actor, string? reason = null)
        {
            if (Status != WalletStatus.Disabled)
                throw new InvalidOperationException($"Cannot enable a wallet that is {Status}");


            bool selfReactivation = actor.IsUser && DisabledByUserId == actor.UserId;

            if (!selfReactivation && !actor.isAdmin)
                throw new UnauthorizedAccessException(
                    $"Actor '{actor.UserId}' lacks authority to re-enable this wallet.");

            var from = Status;
            Status = WalletStatus.Active;
            DisabledByUserId = null;
            Touch();

            RecordHistory(from, Status, actor.UserId, reason);
        }

        public void RequestClosure(WalletActor actor, string? reason = null)
        {
            EnsureActive();
            if (!actor.IsUser || actor.UserId != UserId)
                throw new UnauthorizedAccessException(
                    $"Actor '{actor.UserId}' lacks authority to request closure of this wallet.");

            var from = Status;
            Status = WalletStatus.PendingClosure;
            Touch();

            RecordHistory(from, Status, actor.UserId, reason);
        }

        public void ApproveClosure(WalletActor actor, decimal currentBalance, string? reason = null)
        {
            if (Status != WalletStatus.PendingClosure)
                throw new InvalidOperationException("Wallet must be pending closure before approval");

            EnsureAuthorization(actor, allowSelf: false, allowAdmin: true);

            if (currentBalance != 0m)
                throw new InvalidOperationException("Cannot close wallet. Outstanding balance must be zero.");

            var from = Status;
            Status = WalletStatus.Closed;
            Touch();

            RecordHistory(from, Status, actor.UserId, reason);
        }

        public void RejectClosure(WalletActor actor, string? reason = null)
        {
            if (Status != WalletStatus.PendingClosure)
                throw new InvalidOperationException("Wallet must be in Pending Closure state to reject closure.");

            EnsureAuthorization(actor, allowSelf: false, allowAdmin: true);

            var from = Status;
            Status = WalletStatus.Active;
            Touch();

            RecordHistory(from, Status, actor.UserId, reason);
        }

        public void CloseDirect(WalletActor actor, decimal currentBalance, string? reason = null)
        {
            if (Status != WalletStatus.Active)
                throw new InvalidOperationException("Wallet must be active before closure is successful");

            EnsureAuthorization(actor, allowSelf: false, allowAdmin: true);

            if (currentBalance != 0m)
                throw new InvalidOperationException("Cannot close wallet. Outstanding balance must be zero.");

            var from = Status;
            Status = WalletStatus.Closed;
            Touch();

            RecordHistory(from, Status, actor.UserId, reason);
        }

        // Helpers

        private void EnsureActive()
        {
            if (Status != WalletStatus.Active)
                throw new InvalidOperationException($"Operation is invalid, wallet status is {Status}");
        }

        private void EnsureAuthorization(WalletActor actor, bool allowSelf, bool allowAdmin)
        {
            bool isSelf = allowSelf && actor.IsUser && actor.UserId == UserId;
            bool isAdmin = allowAdmin && actor.isAdmin;

            if (!isSelf && !isAdmin)
                throw new UnauthorizedAccessException(
                    $"Actor '{actor.UserId}' lacks authority to perform this operation.");
        }

        private void RecordHistory(WalletStatus from, WalletStatus to, Guid changedByWho, string? reason)
        {
            _statusHistory.Add(new WalletStatusHistory(AccountNumber, from, to, changedByWho, reason));
        }

        private void Touch() => LastUpdated = DateTime.UtcNow;
    }
}