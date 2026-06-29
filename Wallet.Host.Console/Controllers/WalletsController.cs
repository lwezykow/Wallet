using Microsoft.AspNetCore.Mvc;
using Wallet.Domain.Exceptions;
using Wallet.Domain.Services;
using Wallet.Host.Console.Models;
using Wallet.Host.Console.Services;

namespace Wallet.Host.Console.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WalletsController
    : ControllerBase
{
    private readonly IWalletDataManager _walletService;
    private readonly ILogger<RatesController> _logger;

    public WalletsController(IWalletDataManager walletService, ILogger<RatesController> logger)
    {
        _walletService = walletService;
        _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> Get(uint id)
    {
        try
        {
            _logger.LogInformation($"Getting wallet {id}");
            var wallet = await _walletService.GetWallet(id);
            
            return Ok(wallet.ToModel());
        }
        catch (EntityNotFoundException)
        {
            return  NotFound();
        }
    }
    
    [HttpPost]
    public async Task<ActionResult> Post(CreateWalletModel model)
    {
        try
        {
            _logger.LogInformation($"Creating wallet [{model}]");
            var wallet = (await _walletService.AddWallet(model.ToEntity())).ToModel();
            _logger.LogInformation($"Wallet [{wallet}] has been created]");
            
            return CreatedAtAction(nameof(Get), new { id = wallet.Id }, wallet);
        }
        catch (EntityNotFoundException)
        {
            return  NotFound();
        }
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(uint id)
    {
        try
        {
            _logger.LogInformation($"Removing wallet [{id}]");
            await _walletService.RemoveWalletAsync(id);
            _logger.LogInformation($"Wallet [{id}] was removed");
            
            return Ok();
        }
        catch (EntityNotFoundException)
        {
            return  NotFound();
        }
    }
}