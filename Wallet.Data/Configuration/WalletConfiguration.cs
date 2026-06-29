using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Wallet.Data.Configuration;

public class WalletConfiguration
    : IEntityTypeConfiguration<Wallet.Domain.Entities.Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet.Domain.Entities.Wallet> builder)
    {
        builder.ToTable("Wallet.Wallet")
            .HasKey(m => m.Id);
        
        builder.Property<string>(p => p.Name)
            .IsRequired()
            .HasMaxLength(3);

        builder.Navigation(p => p.Funds)
            .AutoInclude();
    }
}