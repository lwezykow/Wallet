using System.ComponentModel.DataAnnotations;

namespace Wallet.Host.Console.Models;

public class CreateWalletModel
{
    [Required]
    public required string Name { get; init; }

    public override string ToString()
    {
        return $"{nameof(Name)}: {Name}";
    }
}