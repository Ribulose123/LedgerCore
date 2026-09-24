using LedgerCore.Domain.Entities.Wallet;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ledger.Persistence.Configurations
{
    public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder.ToTable("Wallets", schema: "wallets");

            builder.HasKey(w => w.AccountNumber);

            builder.Property(w => w.AccountNumber)
                   .HasMaxLength(10)
                   .IsRequired();

            builder.Property(w => w.UserId)
                   .IsRequired();

            builder.Property(w => w.Currency)
                   .HasMaxLength(3)
                   .IsRequired();

            builder.Property(w => w.Status)
                   .HasConversion<string>()
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(w => w.DisabledByUserId);

            builder.Property(w => w.CreatedAt).IsRequired();
            builder.Property(w => w.LastUpdated).IsRequired();

            builder.HasMany(w => w.StatusHistory)
                   .WithOne()
                   .HasForeignKey(h => h.WalletId)
                   .HasPrincipalKey(w => w.AccountNumber)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(w => w.StatusHistory)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}