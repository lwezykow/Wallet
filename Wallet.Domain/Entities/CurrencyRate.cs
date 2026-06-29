namespace Wallet.Domain.Entities;

public class CurrencyRate : IEntity
{
    public uint Id { get; init; }
    
    public required string Name { get; init; }

    public required string Symbol { get; init; }

    public double Rate { get; set; }

    public bool IsBase { get; init; }
}