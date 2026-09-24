using LedgerCore.Domain.Entities.Wallet;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ledger.Persistence.Configurations
{
    public class WalletStatusHistoryConfiguration:IEntityTypeConfiguration<WalletStatusHistory>
    {
        public void Configure(EntityTypeBuilder<WalletStatusHistory> builder)
        {
            builder.ToTable("WalletStatusHistories", schema: "wallets");

            builder.HasKey(h => h.WalletStatusHistoryId);

            builder.Property(h => h.WalletId)
                .HasConversion<string>()
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(h => h.FromStatus)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(h => h.ToStatus)
               .HasConversion<string>()
               .IsRequired();

            builder.Property(h => h.Reason).HasMaxLength(500);

            builder.Property(h => h.ChangedByWho)
                .IsRequired();

            builder.Property(h => h.CreatedAt).IsRequired();
            
        }
    }
}
