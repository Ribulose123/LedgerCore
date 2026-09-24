using LedgerCore.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ledger.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users", schema: "identity");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Name).HasMaxLength(200).IsRequired();

            builder.Property(u => u.Email).HasMaxLength(320).IsRequired();
            builder.HasIndex(u => u.Email).IsUnique();

            builder.Property(u => u.PasswordHash).IsRequired();

            builder.Property(u => u.Status)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(u => u.CreatedAt).IsRequired();
            builder.Property(u => u.LastUpdated).IsRequired();

            builder.Navigation(u => u.Roles)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}