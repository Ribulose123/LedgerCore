using LedgerCore.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ledger.Persistence.Configurations
{
    public class RoleConfiguration:IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles", schema: "identity");

            builder.HasKey(h => h.Id);

            builder.Property(h => h.Name).HasMaxLength(50).IsRequired();
            builder.HasIndex(h => h.Name).IsUnique();

            builder.HasData(
                new { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "User" },
                new { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Admin" }
                );
        }
    }
}
