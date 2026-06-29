namespace Wallet.Domain.Exceptions;

public class EntityNotFoundException(string message)
    : ApplicationException(message);