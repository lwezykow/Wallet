namespace Wallet.Domain.Exceptions;

public class CurrencyNotSupportedException(string message)
    : ApplicationException(message);