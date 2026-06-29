using System.Diagnostics;
using Microsoft.Extensions.Options;
using Wallet.Domain.Entities;
using Wallet.Domain.Services;
using Wallet.External.Nbp;
using Wallet.External.Nbp.Configuration;

namespace Wallet.Infrastructure;

public class CurrencyRatePoolerService(
    ILogger<CurrencyRatePoolerService> logger,
    ICurrencyRateService currencyRateService,
    IServiceProvider serviceProvider,
    IOptions<NbpCurrencyRatesSettings> settings)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Currency rate pooler starting");
        
        await UpdateCurrencyRates();

        using PeriodicTimer timer = new(TimeSpan.FromSeconds(settings.Value.PoolingInterval));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await UpdateCurrencyRates();
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Currency rate pooler is stopping.");
        }
    }
    
    private async Task UpdateCurrencyRates()
    {
        try
        {
            var start = Stopwatch.GetTimestamp();
            using var scope = serviceProvider.CreateScope();
            var dataManger = scope.ServiceProvider.GetRequiredService<ICurrencyDataManager>();
            
            logger.LogInformation("Querying currency rates");

            var currencies = await currencyRateService.GetRatesAsync();

            logger.LogInformation("Query returned {count} items", currencies.Length);
            logger.LogInformation("Saving items to database");

            int affectedRows = await dataManger.RefreshCurrencyRatesAsync(currencies.ToEntity());
            var delta = Stopwatch.GetElapsedTime(start);

            logger.LogInformation("{affectedRows} currencies loaded in {delta:T} ms", affectedRows, delta);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occured while updating currency rates");
        }
    }   
}