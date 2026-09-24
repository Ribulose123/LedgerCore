using LedgerCore.Domain.Enum;
using LedgerCore.Application.Interface;

namespace Ledger.Persistence.stubs
{
    public class StubUserRoleQuery:IUserRoleQuery
    {
        public Task<ActorRole> GetActorRole(Guid userId) => Task.FromResult(ActorRole.User);
    }
}
