using Wallet.Host.Console.Models;

namespace Wallet.Host.Console.Services;

public static class WalletTranslator
{
    public static Wallet.Domain.Entities.Wallet ToEntity(this CreateWalletModel model)
    {
        return new Wallet.Domain.Entities.Wallet
        {
            Name = model.Name
        };
    }
    
    public static WalletModel ToModel(this Wallet.Domain.Entities.Wallet model)
    {
        return new WalletModel
        {
            Id = model.Id,
            Name = model.Name,
            Funds = model.Funds
                .Where(i => i.Money.Amount > 0)
                .Select(i => i.Money.ToModel())
                .ToArray()
        };
    }
}