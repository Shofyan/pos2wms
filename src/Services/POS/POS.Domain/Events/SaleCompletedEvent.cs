using POS.Domain.Common;

namespace POS.Domain.Events;

/// <summary>
/// Event raised when a sale is completed
/// </summary>
public sealed record SaleCompletedEvent : DomainEvent
{
    public override string EventType => "pos.sale.completed";

    public Guid SaleId { get; init; }
    public string TransactionNumber { get; init; }
    public string StoreId { get; init; }
    public decimal TotalAmount { get; init; }
    public IReadOnlyList<ItemInfo> Items { get; init; }

    public SaleCompletedEvent(
        Guid saleId,
        string transactionNumber,
        string storeId,
        decimal totalAmount,
        IReadOnlyList<ItemInfo> items)
    {
        SaleId = saleId;
        TransactionNumber = transactionNumber;
        StoreId = storeId;
        TotalAmount = totalAmount;
        Items = items;
    }

    public sealed record ItemInfo(
        string Sku,
        int Quantity,
        string? WarehouseId,
        string? LocationId);
}
