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

    public static IEnumerable<WalletModel> ToModel(this IEnumerable<Wallet.Domain.Entities.Wallet> entities)
    {
        return entities.Select(ToModel);
    }
    
    public static WalletModel ToModel(this Wallet.Domain.Entities.Wallet entity)
    {
        return new WalletModel
        {
            Id = entity.Id,
            Name = entity.Name,
            Funds = entity.Funds
                .Where(i => i.Money.Amount > 0)
                .Select(i => i.Money.ToModel())
                .ToArray()
        };
    }
}