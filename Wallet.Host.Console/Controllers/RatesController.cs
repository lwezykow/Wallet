using Microsoft.AspNetCore.Mvc;
using Wallet.External.Nbp;

namespace Wallet.Host.Console.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RatesController
    : ControllerBase
{
    private readonly ICurrencyRateService _currencyRateService;
    private readonly ILogger<RatesController> _logger;

    public RatesController(ICurrencyRateService currencyRateService, ILogger<RatesController> logger)
    {
        _currencyRateService = currencyRateService;
        _logger = logger;
    }

    [HttpGet(Name = "Load")]
    public async Task<ActionResult> Get()
    {
        return Ok(await _currencyRateService.GetRatesAsync());
    }
}