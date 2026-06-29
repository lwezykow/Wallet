using Wallet.Domain.ValueTypes;
using Wallet.Host.Console.Models;

namespace Wallet.Host.Console.Services;

public static class BuyCurrencyRequestTranslator
{
    public static ExchangeCurrencyRequest ToEntity(this BuyCurrencyRequestModel model)
    {
        return new ExchangeCurrencyRequest
        {
            Money = model.Target.ToEntity(),
            Currency = model.SourceCurrency.ToUpper()
        };
    }
}