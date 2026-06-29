using Wallet.External.Nbp.Config;

namespace Wallet.External.Nbp;

public static class RegisterDependencies
{
    public static void AddNbpCurrencyRates(this IServiceCollection services)
    {
        services.AddOptions<NbpCurrencyRatesSettings>()
            .BindConfiguration(NbpCurrencyRatesSettings.SectionName)
            .ValidateOnStart();
        
        services.AddHttpClient<ICurrencyRateService, CurrencyRateService>();
        services.AddSingleton<ICurrencyRateService, CurrencyRateService>();
    }
}