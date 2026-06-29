using Wallet.Domain.Entities;

namespace Wallet.Domain.Repositories;

public interface ICurrencyRateRepository
    : IRepository<CurrencyRate>
{
    public Dictionary<string, CurrencyRate> GetCurrencyRatesDictionary();
}

