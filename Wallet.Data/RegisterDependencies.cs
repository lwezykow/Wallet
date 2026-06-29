using Wallet.Data.Configuration;
using Wallet.Data.Repositories;
using Wallet.Domain.Repositories;
using Wallet.Domain.Services;

namespace Wallet.Data;

public static class RegisterDependencies
{
    public static void AddWalletData(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddOptions<DatabaseSettings>()
            .BindConfiguration(DatabaseSettings.SectionName)
            .ValidateOnStart();

        services.AddDbContext<WalletDbContext>(opt =>
        {
            opt.LogTo(Console.WriteLine, LogLevel.Information);
        });
        
        services.AddScoped<ICurrencyRateRepository, CurrencyRateRepository>();
        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
        services.AddScoped<ICurrencyDataManager, CurrencyDataManager>();
        services.AddScoped<IWalletDataManager, WalletDataManager>();
    }
}