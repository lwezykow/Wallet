using Microsoft.Extensions.Options;
using Wallet.External.Nbp.Config;
using Wallet.External.Nbp.Dto;

namespace Wallet.External.Nbp;

public class CurrencyRateService
    : ICurrencyRateService
{
    private readonly HttpClient _httpClient;
    private readonly NbpCurrencyRatesSettings _settings;
    private readonly ILogger<CurrencyRateService> _logger;

    public CurrencyRateService(HttpClient httpClient, IOptions<NbpCurrencyRatesSettings> settings, ILogger<CurrencyRateService> logger)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<NbpRate[]> GetRatesAsync()
    {
        _logger.LogDebug("Getting json data");
        
        var tables = await _httpClient.GetFromJsonAsync<NbpTable[]>(_settings.ApiUri);

        if (tables == null || tables.Length == 0)
        {
            _logger.LogWarning("No tables were returned by the API request");
            return Array.Empty<NbpRate>();
        }
        
        _logger.LogDebug("Number of tables returned: {0}", tables.Length);
        _logger.LogDebug("Number of rates returned: {0}", tables[0].rates.Count);

        return tables[0].rates.ToArray();
    }
}