

using LedgerCore.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ledger.Persistence.Configurations
{
    public class UserRoleConfiguration:IEntityTypeConfiguration<UserRole>
   {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("UserRoles", schema: "identity");

            builder.HasKey(ur => ur.Id);


            builder.Property(ur => ur.AssignedAt).IsRequired();
            builder.Property(ur => ur.AssignedBy).HasMaxLength(200).IsRequired();


            builder.HasIndex(ur => new { ur.UserId, ur.RoleId });
            builder.HasOne<User>()
                  .WithMany(u => u.Roles)
                  .HasForeignKey(ur => ur.UserId)
                  .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Role>()
                   .WithMany()
                   .HasForeignKey(ur => ur.RoleId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
