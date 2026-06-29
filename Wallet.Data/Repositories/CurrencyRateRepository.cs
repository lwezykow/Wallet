using Microsoft.Extensions.Caching.Memory;
using Wallet.Domain.Entities;
using Wallet.Domain.Repositories;

namespace Wallet.Data.Repositories;

public class CurrencyRateRepository(WalletDbContext context, IMemoryCache cache, ILogger<GenericRepository<CurrencyRate>> logger)
    : GenericRepository<CurrencyRate>(context), ICurrencyRateRepository
{
    private readonly ILogger<GenericRepository<CurrencyRate>> _logger = logger;
    
    private const string CurrencyRatesCacheKey = "CurrencyRates";
    
    public Dictionary<string, CurrencyRate> GetCurrencyRatesDictionary()
    {
        var currencyRates = cache.Get<Dictionary<string, CurrencyRate>>(CurrencyRatesCacheKey);
        if (currencyRates != null)
        {
            return currencyRates;
        }
        
        _logger.LogInformation("Refreshing currency rates cache");
            
        currencyRates = GetAllAsync().GetAwaiter().GetResult().ToDictionary(r => r.Symbol, r => r);
        cache.Set(CurrencyRatesCacheKey, currencyRates, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(30)));

        return currencyRates;
    }
}