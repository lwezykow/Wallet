using System.ComponentModel.DataAnnotations;

namespace Wallet.Host.Console.Models;

public class BuyCurrencyRequestModel
{
    [Required]
    public required MoneyModel Target { get; set; }
    
    [Required]
    public required string SourceCurrency { get; set; }

    public override string ToString()
    {
        return $"{nameof(Target)}: {Target}, {nameof(SourceCurrency)}: {SourceCurrency}";
    }
}