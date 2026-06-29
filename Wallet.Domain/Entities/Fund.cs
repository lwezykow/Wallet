using Wallet.Domain.ValueTypes;

namespace Wallet.Domain.Entities;

public class Fund 
    : IEntity
{
    public uint Id { get; init; }
    
    public required Money Money { get; set; }
    
    public required Wallet Wallet { get; init; }
}