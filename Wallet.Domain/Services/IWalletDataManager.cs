using Wallet.Domain.ValueTypes;

namespace Wallet.Domain.Services;

public interface IWalletDataManager
{
    Task<Entities.Wallet> GetWallet(uint id);
    
    Task<Entities.Wallet> AddWallet(Entities.Wallet wallet);
    
    Task RemoveWalletAsync(uint id);
    
    Task<Entities.Wallet> DepositAsync(uint walletId, Money deposit);

    Task<Entities.Wallet> WithdrawAsync(uint walletId, Money withdrawal);
    
    Task<Entities.Wallet> SellCurrencyAsync(uint walletId, ExchangeCurrencyRequest request);
    
    Task<Entities.Wallet> BuyCurrencyAsync(uint walletId, ExchangeCurrencyRequest request);
    
    Task<IEnumerable<Entities.Wallet>> GetWallets();
}