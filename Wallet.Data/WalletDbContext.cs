using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Wallet.Data.Configuration;
using Wallet.Domain.Entities;

namespace Wallet.Data;

public class WalletDbContext(IOptions<DatabaseSettings> settings) : DbContext
{
    private readonly DatabaseSettings _settings = settings.Value;
    
    public DbSet<CurrencyRate> CurrencyRates { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(_settings.ConnectionString);
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(WalletDbContext).Assembly);
        
        base.OnModelCreating(builder);
    }
}