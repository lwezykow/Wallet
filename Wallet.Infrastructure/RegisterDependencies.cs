namespace Wallet.Infrastructure;

public static class RegisterDependencies
{
    public static void AddRatesPooler(this IServiceCollection services)
    {
        services.AddHostedService<CurrencyRatePoolerService>();
    }
}