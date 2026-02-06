namespace POS.Domain.Exceptions;

/// <summary>
/// Exception for invalid sale operations
/// </summary>
public sealed class InvalidSaleException : DomainException
{
    public InvalidSaleException(string message)
        : base("INVALID_SALE", message)
    {
    }
}
