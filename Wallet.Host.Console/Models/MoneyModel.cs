using System.ComponentModel.DataAnnotations;

namespace Wallet.Host.Console.Models;

public class MoneyModel
{
    [Required]
    [Range(double.Epsilon, double.MaxValue)]
    public double Amount { get; init; }

    [Required]
    public required string Currency { get; init; }

    public override string ToString()
    {
        return $"{nameof(Amount)}: {Amount}, {nameof(Currency)}: {Currency}";
    }

    private bool Equals(MoneyModel other)
    {
        return Amount.Equals(other.Amount) && Currency == other.Currency;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((MoneyModel)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Amount, Currency);
    }

}