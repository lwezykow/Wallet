using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wallet.Domain.Entities;

namespace Wallet.Data.Configuration;

public class CurrencyRateConfiguration
    : IEntityTypeConfiguration<CurrencyRate>
{
    public void Configure(EntityTypeBuilder<CurrencyRate> builder)
    {
        builder.ToTable("Wallet.CurrencyRates")
            .HasKey(m => m.Id);
        
        builder.Property<string>(p => p.Symbol)
            .IsRequired()
            .HasMaxLength(3);
        
        builder.Property<string>(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(p => p.IsBase)
            .HasDefaultValue(false);
        
        builder.Property(p => p.IsBase)
            .HasDefaultValue(false);
    }
}