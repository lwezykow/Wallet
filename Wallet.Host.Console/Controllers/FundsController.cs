using Microsoft.AspNetCore.Mvc;
using Wallet.Domain.Exceptions;
using Wallet.Domain.Services;
using Wallet.Host.Console.Models;
using Wallet.Host.Console.Services;

namespace Wallet.Host.Console.Controllers;

[ApiController]
[Route("api/Wallets/{walletId}/")]
public class FundsController(IWalletDataManager walletService, ILogger<RatesController> logger)
    : ControllerBase
{
    [HttpPost("Deposit")]
    public async Task<ActionResult> Deposit(uint walletId, MoneyModel money)
    {
        try
        {
            logger.LogInformation("Depositing [{deposit}] to wallet id=[{walletId}]", money, walletId);
            var wallet = (await walletService.DepositAsync(walletId, money.ToEntity())).ToModel();
            logger.LogInformation("Money was deposited to wallet [{walletId}]", walletId);
            
            return Ok(wallet);
        }
        catch (EntityNotFoundException e)
        {
            return  NotFound(e.Message);
        }
        catch (CurrencyNotSupportedException e)
        {
            return  BadRequest(e.Message);
        }
    }
    
    [HttpPost("SellCurrency")]
    public async Task<ActionResult> SellCurrency(uint walletId, SellCurrencyRequestModel sellCurrencyRequest)
    {
        try
        {
            logger.LogInformation("Executing sell currency request [{exchangeRequest}] for wallet id=[{walletId}]", sellCurrencyRequest, walletId);
            var wallet = (await walletService.SellCurrencyAsync(walletId, sellCurrencyRequest.ToEntity())).ToModel();
            logger.LogInformation("Currency sold for [{walletId}]", walletId);
            
            return Ok(wallet);
        }
        catch (EntityNotFoundException e)
        {
            return  NotFound(e.Message);
        }
        catch (CurrencyNotSupportedException e)
        {
            return  BadRequest(e.Message);
        }
    }
    
    [HttpPost("BuyCurrency")]
    public async Task<ActionResult> BuyCurrency(uint walletId, BuyCurrencyRequestModel buyCurrencyRequest)
    {
        try
        {
            logger.LogInformation("Executing buy currency request [{exchangeRequest}] for wallet id=[{walletId}]", buyCurrencyRequest, walletId);
            var wallet = (await walletService.BuyCurrencyAsync(walletId, buyCurrencyRequest.ToEntity())).ToModel();
            logger.LogInformation("Currency bought for [{walletId}]", walletId);
            
            return Ok(wallet);
        }
        catch (EntityNotFoundException e)
        {
            return  NotFound(e.Message);
        }
        catch (CurrencyNotSupportedException e)
        {
            return  BadRequest(e.Message);
        }
    }
    
    [HttpPost("Withdraw")]
    public async Task<ActionResult> Withdraw(uint walletId, MoneyModel withdraw)
    {
        try
        {
            logger.LogInformation("Withdrawing [{withdraw}] from wallet id=[{walletId}]", withdraw, walletId);
            var wallet = (await walletService.WithdrawAsync(walletId, withdraw.ToEntity())).ToModel();
            logger.LogInformation($"Money was withdrawn from wallet [{walletId}]", walletId);
            
            return Ok(wallet);
        }
        catch (EntityNotFoundException e)
        {
            return  NotFound(e.Message);
        }
        catch (InsufficientFundsException e)
        {
            return  BadRequest(e.Message);
        }
    }
}