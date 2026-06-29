using Wallet.Domain.ValueTypes;
using Wallet.Host.Console.Models;

namespace Wallet.Host.Console.Services;

public static class SellCurrencyRequestTranslator
{
    public static ExchangeCurrencyRequest ToEntity(this SellCurrencyRequestModel model)
    {
        return new ExchangeCurrencyRequest
        {
            Money = model.Source.ToEntity(),
            Currency = model.TargetCurrency.ToUpper()
        };
    }
}