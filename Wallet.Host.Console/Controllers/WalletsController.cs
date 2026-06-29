using Microsoft.AspNetCore.Mvc;
using Wallet.Domain.Exceptions;
using Wallet.Domain.Services;
using Wallet.Host.Console.Models;
using Wallet.Host.Console.Services;

namespace Wallet.Host.Console.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WalletsController(IWalletDataManager walletService, ILogger<RatesController> logger)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> Get()
    {
        try
        {
            logger.LogInformation("Getting all wallets");
            var wallet = await walletService.GetWallets();
            
            return Ok(wallet.ToModel());
        }
        catch (EntityNotFoundException)
        {
            return  NotFound();
        }
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult> Get(uint id)
    {
        try
        {
            logger.LogInformation("Getting wallet {id}", id);
            var wallet = await walletService.GetWallet(id);
            
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
            logger.LogInformation("Creating wallet [{createWalletModel}]", model);
            var wallet = (await walletService.AddWallet(model.ToEntity())).ToModel();
            logger.LogInformation("Wallet [{walletModel}] has been created]", wallet);
            
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
            logger.LogInformation("Removing wallet [{id}]", id);
            await walletService.RemoveWalletAsync(id);
            logger.LogInformation("Wallet [{id}] was removed", id);
            
            return Ok();
        }
        catch (EntityNotFoundException)
        {
            return  NotFound();
        }
    }
}