using System.ComponentModel.DataAnnotations;

namespace Wallet.Host.Console.Models;

public class SellCurrencyRequestModel
{
    [Required]
    public required MoneyModel Source { get; init; }
    
    [Required]
    public required string TargetCurrency { get; init; }

    public override string ToString()
    {
        return $"{nameof(Source)}: {Source}, {nameof(TargetCurrency)}: {TargetCurrency}";
    }
}