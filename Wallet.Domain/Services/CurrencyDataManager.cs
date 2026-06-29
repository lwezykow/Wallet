using Wallet.Domain.Entities;
using Wallet.Domain.Exceptions;
using Wallet.Domain.Repositories;
using Wallet.Domain.ValueTypes;

namespace Wallet.Domain.Services;

public class CurrencyDataManager(ICurrencyRateRepository repository)
    : ICurrencyDataManager
{
    private static readonly CurrencyRate BaseCurrency =
        new()
        { 
            Name = "Polski Nowy Złoty",
            Symbol = "PLN",
            IsBase = true,
            Rate = 1.0 
        };
    
    public async Task<int> RefreshCurrencyRatesAsync(IEnumerable<CurrencyRate> rates)
    {
        var currentRates = (await repository.GetAllAsync()).ToDictionary(r => r.Symbol, r => r);
            
        foreach (var rate in rates)
        {
            if (currentRates.TryGetValue(rate.Symbol, out var currentRate))
            {
                currentRate.Rate = rate.Rate;
            }
            else
            {
                repository.Add(
                    new CurrencyRate
                    {
                        Symbol = rate.Symbol, 
                        Name = rate.Name, 
                        Rate = rate.Rate
                    });
            }
        }

        if (!currentRates.ContainsKey(BaseCurrency.Symbol))
        {
            repository.Add(BaseCurrency);
        }
        
        return await repository.SaveChangesAsync();
    }

    public Money ConvertCurrency(Money money, string targetCurrency)
    {
        if (money.Currency == targetCurrency)
        {
            return money;
        }
        
        var targetRate = EnsureCurrencyRate(targetCurrency);
        var sourceRate = EnsureCurrencyRate(money.Currency);

        var value = ConvertFromBase(ConvertToBase(money.Amount, sourceRate), targetRate); 
        
        return new Money(amount: value, currency: targetCurrency);
    }

    public void ValidateCurrency(string currency)
    {
        var currencyRates = repository.GetCurrencyRatesDictionary();
        
        if (!currencyRates.ContainsKey(currency))
        {
            throw new CurrencyNotSupportedException($"Currency {currency} is not supported!");
        }
    }
    
    private static double ConvertToBase(double value, CurrencyRate rate)
    {
        return value * rate.Rate;
    }
    
    private static double ConvertFromBase(double value, CurrencyRate rate)
    {
        return value / rate.Rate;
    }

    private CurrencyRate EnsureCurrencyRate(string currency)
    {
        var currencyRates = repository.GetCurrencyRatesDictionary();
        
        return currencyRates.TryGetValue(currency, out var rate) ? rate 
            : throw new CurrencyNotSupportedException($"Currency {currency} is not supported!");
    }
}