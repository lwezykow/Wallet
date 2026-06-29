using Wallet.Domain.Services;

namespace Wallet.Domain;

public static class RegisterDependencies
{
    public static void AddDomain(this IServiceCollection services)
    {
        services.AddScoped<ICurrencyDataManager, CurrencyDataManager>();
    }
}