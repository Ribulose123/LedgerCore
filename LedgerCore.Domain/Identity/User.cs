using LedgerCore.Domain.Enum;

namespace LedgerCore.Domain.Identity
{
    public class User
    {
        private readonly List<UserRole> _roles = new();

        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public UserStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime LastUpdated { get; private set; }

        public IReadOnlyCollection<UserRole> Roles => _roles.AsReadOnly();

        
        private User() { }

        public User(string name, string email, string passwordHash, Guid defaultRoleId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name is required.", nameof(name));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password is required.", nameof(passwordHash));

            if (defaultRoleId == Guid.Empty)
                throw new ArgumentException("Default Role ID cannot be empty.", nameof(defaultRoleId));

            Id = Guid.NewGuid();
            Name = name;
            Email = email;
            PasswordHash = passwordHash;
            Status = UserStatus.Active;
            CreatedAt = DateTime.UtcNow;
            LastUpdated = DateTime.UtcNow;

            
            var defaultRole = new UserRole(Id, defaultRoleId, assignedBy: "System");
            _roles.Add(defaultRole);
        }

        public void UpdateStatus(UserStatus status)
        {
            Status = status;
            LastUpdated = DateTime.UtcNow;
        }

        public void AddRole(Guid roleId, string assignedBy)
        {
            if (roleId == Guid.Empty)
                throw new ArgumentException("Role ID cannot be empty.", nameof(roleId));

            if (_roles.Exists(r => r.RoleId == roleId))
                return; 

            _roles.Add(new UserRole(Id, roleId, assignedBy));
            LastUpdated = DateTime.UtcNow;
        }

        public Guid GetCurrentRoleId()
        {
            if(Roles.Count == 0)
            {
                throw new InvalidOperationException("User must be assign to a role");
            }

            return Roles.OrderByDescending(r => r.AssignedAt)
                .First()
                .RoleId;
        }
    }
}