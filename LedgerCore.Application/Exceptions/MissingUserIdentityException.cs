

namespace LedgerCore.Application.Exceptions
{
    public enum IdentityFailureReason { NoHttpContext, NotAuthenticated, InvalidClaim }

    public sealed class MissingUserIdentityException : Exception
    {
        public IdentityFailureReason Reason { get; }

        public MissingUserIdentityException(IdentityFailureReason reason)
            : base("The authenticated user identity is missing or invalid.")
            => Reason = reason;
    }
}
