using Wallet.External.Nbp.Dto;

namespace Wallet.External.Nbp;

public interface ICurrencyRateService
{
    Task<NbpRate[]> GetRatesAsync();
}