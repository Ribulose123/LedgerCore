
using LedgerCore.Application.Exceptions;
using LedgerCore.Application.Interface;
using LedgerCore.Application.Constants;

namespace LedgerCore.Api.Security
{

    public sealed class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _accessor;
        public CurrentUser(IHttpContextAccessor accessor) => _accessor = accessor;

        public Guid UserId => GetRequiredGuidClaim(JwtClaimTypes.Subject);
        public Guid RoleId => GetRequiredGuidClaim(JwtClaimTypes.RoleId);

        private Guid GetRequiredGuidClaim(string claimType)
        {
            var principal = _accessor.HttpContext?.User
                ?? throw new MissingUserIdentityException(IdentityFailureReason.NoHttpContext);

            if (principal.Identity?.IsAuthenticated != true)
                throw new MissingUserIdentityException(IdentityFailureReason.NotAuthenticated);

            var value = principal.FindFirst(claimType)?.Value;

            if (!Guid.TryParse(value, out var id) || id == Guid.Empty)
                throw new MissingUserIdentityException(IdentityFailureReason.InvalidClaim);

            return id;
        }
    }
}
