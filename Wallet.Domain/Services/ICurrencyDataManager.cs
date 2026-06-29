using Wallet.Domain.Entities;
using Wallet.Domain.ValueTypes;

namespace Wallet.Domain.Services;

public interface ICurrencyDataManager
{
    Task<int> RefreshCurrencyRatesAsync(IEnumerable<CurrencyRate> rates);
    
    Money ConvertCurrency(Money money, string targetCurrency);
    
    void ValidateCurrency(string currency);
}