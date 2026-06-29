using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wallet.Domain.Entities;

namespace Wallet.Data.Configuration;

public class FundConfiguration
    : IEntityTypeConfiguration<Fund>
{
    public void Configure(EntityTypeBuilder<Fund> builder)
    {
        builder.ToTable("Wallet.Funds")
            .HasKey(m => m.Id);
        
        builder.OwnsOne(f => f.Money, money =>
        {
            money.Property(m => m.Amount).HasColumnName("Amount").IsRequired();
            money.Property(m => m.Currency).HasColumnName("Currency").IsRequired();
        });

        builder.HasOne(p => p.Wallet);
    }
}