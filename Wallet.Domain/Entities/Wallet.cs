namespace Wallet.Domain.Entities;

public class Wallet
    : IEntity
{
    public uint Id { get; init; }
    
    public required string Name { get; init; }

    public List<Fund> Funds { get; init; } = [];
}