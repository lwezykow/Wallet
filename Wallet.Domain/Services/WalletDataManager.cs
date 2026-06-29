using Wallet.Domain.Entities;
using Wallet.Domain.Exceptions;
using Wallet.Domain.Repositories;
using Wallet.Domain.ValueTypes;

namespace Wallet.Domain.Services;

public class WalletDataManager(
    ICurrencyDataManager currencyDataManager,
    IRepository<Entities.Wallet> walletRepository)
    : IWalletDataManager
{
    public async Task<Entities.Wallet> GetWallet(uint id)
    {
        return await EnsureWallet(id);
    }

    public async Task<Entities.Wallet> AddWallet(Entities.Wallet wallet)
    {
        walletRepository.Add(wallet);
        await walletRepository.SaveChangesAsync();

        return wallet;
    }
    
    public async Task RemoveWalletAsync(uint id)
    {
        var wallet = await EnsureWallet(id);
        
        walletRepository.Remove(wallet);
        await walletRepository.SaveChangesAsync();
    }
    
    public async Task<Entities.Wallet> DepositAsync(uint walletId, Money money)
    {
        currencyDataManager.ValidateCurrency(money.Currency);
        
        var wallet = await EnsureWallet(walletId);

        DepositFunds(wallet, money);
        
        await walletRepository.SaveChangesAsync();

        return wallet;
    }

    public async Task<Entities.Wallet> WithdrawAsync(uint walletId, Money money)
    {
        var wallet = await EnsureWallet(walletId);
        var fund = EnsureFunds(wallet, money);
        
        fund.Money -= money;

        await walletRepository.SaveChangesAsync();
        
        return wallet;
    }

    public async Task<Entities.Wallet> SellCurrencyAsync(uint walletId, ExchangeCurrencyRequest request)
    {
        var wallet = await EnsureWallet(walletId);

        if (request.Currency != request.Money.Currency)
        {
            currencyDataManager.ValidateCurrency(request.Currency);
            currencyDataManager.ValidateCurrency(request.Money.Currency);

            var sourceFund = EnsureFunds(wallet, request.Money);
            var exchangedFund = ExchangeFunds(sourceFund, request);
            
            wallet.Funds.Add(exchangedFund);

            await walletRepository.SaveChangesAsync();
        }

        return wallet;
    }

    public async Task<Entities.Wallet> BuyCurrencyAsync(uint walletId, ExchangeCurrencyRequest request)
    {
        var wallet = await EnsureWallet(walletId);

        if (request.Money.Currency != request.Currency)
        {
            currencyDataManager.ValidateCurrency(request.Money.Currency);
            currencyDataManager.ValidateCurrency(request.Currency);
            
            Money requiredFunds = currencyDataManager.ConvertCurrency(request.Money, request.Currency);
            
            var sourceFund = EnsureFunds(wallet, requiredFunds);
            sourceFund.Money -= requiredFunds; 
            
            wallet.Funds.Add(new Fund { Wallet = wallet, Money = request.Money });
            
            await walletRepository.SaveChangesAsync();
        }

        return wallet;
    }

    public async Task<IEnumerable<Entities.Wallet>> GetWallets()
    {
        return await walletRepository.GetAllAsync();
    }

    private Fund ExchangeFunds(Fund sourceFund, ExchangeCurrencyRequest request)
    {
        sourceFund.Money -= request.Money;
        
        Money exchangedMoney = currencyDataManager.ConvertCurrency(request.Money, request.Currency);
        
        return new Fund { Wallet = sourceFund.Wallet, Money = exchangedMoney };
    }

    private Fund EnsureFunds(Entities.Wallet wallet, Money source)
    {
        foreach (var fund in wallet.Funds)
        {
            if (string.Equals(fund.Money.Currency, source.Currency, StringComparison.CurrentCultureIgnoreCase)
                && fund.Money.Amount >= source.Amount)
            {
                return fund;
            }
        }
        
        throw new InsufficientFundsException($"Insufficient funds for currency. Required amount [{source}] was available in the wallet!");
    }

    private void DepositFunds(Entities.Wallet wallet, Money money)
    {
        foreach (var fund in wallet.Funds)
        {
            if (string.Equals(fund.Money.Currency, money.Currency, StringComparison.CurrentCultureIgnoreCase))
            {
                fund.Money += money;
        
                return;
            }
        }
        
        wallet.Funds.Add(new Fund { Wallet = wallet, Money = money });
    }

    private async Task<Entities.Wallet> EnsureWallet(uint id)
    {
        return await walletRepository.GetSingleOrDefaultAsync(id)
               ?? throw new EntityNotFoundException($"Wallet id=[{id}] was not found");
    }
}