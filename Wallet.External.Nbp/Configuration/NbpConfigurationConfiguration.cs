namespace Wallet.External.Nbp.Configuration;

public class NbpCurrencyRatesSettings
{
    public const string SectionName = "Nbp.CurrencyRates.Service";
    
    public string ApiUri { get; set; }

    public int PoolingInterval { get; set; }
}