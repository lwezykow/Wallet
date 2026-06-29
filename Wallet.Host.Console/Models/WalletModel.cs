namespace Wallet.Host.Console.Models;

public class WalletModel
{
    public uint Id { get; init; }

    public required string Name { get; init; }

    public required MoneyModel[]  Funds { get; init; }
    
    public override string ToString()
    {
        return $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}";
    }
}