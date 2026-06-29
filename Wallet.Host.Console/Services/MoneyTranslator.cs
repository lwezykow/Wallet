using Wallet.Domain.ValueTypes;
using Wallet.Host.Console.Models;

namespace Wallet.Host.Console.Services;

public static class MoneyTranslator
{
    public static Money ToEntity(this MoneyModel model)
    {
        return new Money(amount: model.Amount, currency: model.Currency.ToUpper());
    }
    
    public static MoneyModel ToModel(this Money entity)
    {
        return new MoneyModel
        {
            Amount = entity.Amount,
            Currency = entity.Currency
        };
    }
}