using System;

namespace LedgerCore.Domain.Identity
{
    public class Role
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;

        private Role() { }

        public Role(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Role name is required.", nameof(name));

            Id = Guid.NewGuid();
            Name = name;
        }
    }
}