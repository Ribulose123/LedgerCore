public class UserRole
{
    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }

    public DateTime AssignedAt { get; private set; }

    public string AssignedBy { get; private set; } = string.Empty;

    private UserRole() { }

    public UserRole(
        Guid userId,
        Guid roleId,
        string assignedBy)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(
                "User ID cannot be empty.",
                nameof(userId));

        if (roleId == Guid.Empty)
            throw new ArgumentException(
                "Role ID cannot be empty.",
                nameof(roleId));

        if (string.IsNullOrWhiteSpace(assignedBy))
            throw new ArgumentException(
                "Assigned by user cannot be empty.",
                nameof(assignedBy));

        Id = Guid.NewGuid();

        UserId = userId;
        RoleId = roleId;
        AssignedAt = DateTime.UtcNow;
        AssignedBy = assignedBy;
    }
}