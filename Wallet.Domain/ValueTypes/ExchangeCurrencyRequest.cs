namespace Wallet.Domain.ValueTypes;

public class ExchangeCurrencyRequest
{
    public required Money Money { get; init; }

    public required string Currency { get; init; }
}