using LedgerCore.Application.Constants;
using LedgerCore.Domain.Identity;
using Microsoft.AspNetCore.Authorization;

namespace LedgerCore.Authorization
{
    public sealed class AdminRoleHandler : AuthorizationHandler<AdminRoleRequirement> 
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRoleRequirement requirement)
        {
            var roleIdClaim = context.User.FindFirst(JwtClaimTypes.RoleId);

            if(roleIdClaim is null)
            {
                return Task.CompletedTask;
            }

            if(!Guid.TryParse(roleIdClaim.Value, out var roleId))
            {
               return Task.CompletedTask;
            }

            if (roleId == RoleIds.Admin)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
