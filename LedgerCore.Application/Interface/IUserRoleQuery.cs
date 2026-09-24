using LedgerCore.Domain.Enum;

namespace LedgerCore.Application.Interface
{
    public interface IUserRoleQuery
    {
        Task<ActorRole> GetActorRole(Guid userId);
    }
}
