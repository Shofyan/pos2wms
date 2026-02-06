namespace POS.Domain.Exceptions;

/// <summary>
/// Exception for invalid return operations
/// </summary>
public sealed class InvalidReturnException : DomainException
{
    public InvalidReturnException(string message)
        : base("INVALID_RETURN", message)
    {
    }
}
