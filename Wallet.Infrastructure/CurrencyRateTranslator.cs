using Wallet.Domain.Entities;
using Wallet.External.Nbp.Dto;

namespace Wallet.Infrastructure;

public static class CurrencyRateTranslator
{
    public static IEnumerable<CurrencyRate> ToEntity(this IEnumerable<NbpRate> rates)
    {
        return rates.Select(ToEntity);
    }
    
    private static CurrencyRate ToEntity(this NbpRate rate)
    {
        return new CurrencyRate { Symbol = rate.code, Name = rate.currency, Rate = rate.mid };
    }
}