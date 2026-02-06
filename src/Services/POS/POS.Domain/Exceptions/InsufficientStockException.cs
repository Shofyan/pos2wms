namespace POS.Domain.Exceptions;

/// <summary>
/// Exception for insufficient stock
/// </summary>
public sealed class InsufficientStockException : DomainException
{
    public string Sku { get; }
    public int RequestedQuantity { get; }
    public int AvailableQuantity { get; }

    public InsufficientStockException(string sku, int requestedQuantity, int availableQuantity)
        : base("INSUFFICIENT_STOCK",
            $"Insufficient stock for SKU {sku}. Requested: {requestedQuantity}, Available: {availableQuantity}")
    {
        Sku = sku;
        RequestedQuantity = requestedQuantity;
        AvailableQuantity = availableQuantity;
    }
}
