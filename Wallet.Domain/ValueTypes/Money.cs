namespace Wallet.Domain.ValueTypes;

public class Money(double amount, string currency)
{
    public double Amount { get; } = amount;

    public string Currency { get; } = currency;

    public override string ToString()
    {
        return $"{Amount:F} {Currency}";
    }
    
    public static Money operator +(Money a, Money b)
    {
        if (a.Currency != b.Currency)
        {
            throw new InvalidOperationException("Cannot add different currencies");
        }
        
        return new Money(a.Amount + b.Amount, a.Currency);
    }

    public static Money operator -(Money a, Money b)
    {
        if (a.Currency != b.Currency)
        {
            throw new InvalidOperationException("Cannot subtract different currencies");
        }
        
        return new Money(a.Amount - b.Amount, a.Currency);
    }
}