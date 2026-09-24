using LedgerCore.Domain.Identity;

namespace LedgerCore.Application.Dtos.UsersDto
{
    public record UserResponse(
        Guid Id,
        string Name,
        string Email,
        string Status,
        DateTime CreatedAt)
    {
        public static UserResponse FromDomain(User user) =>
            new(
                user.Id,
                user.Name,
                user.Email,
                user.Status.ToString(),
                user.CreatedAt);
    }
}