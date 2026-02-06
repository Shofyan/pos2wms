using POS.Domain.Common;

namespace POS.Domain.Events;

/// <summary>
/// Event raised when a sale is cancelled
/// </summary>
public sealed record SaleCancelledEvent : DomainEvent
{
    public override string EventType => "pos.sale.cancelled";

    public Guid SaleId { get; init; }
    public string TransactionNumber { get; init; }
    public string StoreId { get; init; }
    public string Reason { get; init; }
    public string? AuthorizedBy { get; init; }
    public bool WasCompleted { get; init; }
    public IReadOnlyList<ItemInfo> Items { get; init; }

    public SaleCancelledEvent(
        Guid saleId,
        string transactionNumber,
        string storeId,
        string reason,
        string? authorizedBy,
        bool wasCompleted,
        IReadOnlyList<ItemInfo> items)
    {
        SaleId = saleId;
        TransactionNumber = transactionNumber;
        StoreId = storeId;
        Reason = reason;
        AuthorizedBy = authorizedBy;
        WasCompleted = wasCompleted;
        Items = items;
    }

    public sealed record ItemInfo(
        string Sku,
        int Quantity,
        string? WarehouseId,
        string? LocationId);
}
