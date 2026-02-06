using POS.Domain.Common;

namespace POS.Domain.Events;

/// <summary>
/// Event raised when a return is created/processed
/// </summary>
public sealed record ReturnCreatedEvent : DomainEvent
{
    public override string EventType => "pos.return.created";

    public Guid ReturnId { get; init; }
    public string ReturnNumber { get; init; }
    public Guid OriginalSaleId { get; init; }
    public string OriginalTransactionNumber { get; init; }
    public string StoreId { get; init; }
    public decimal RefundAmount { get; init; }
    public string RefundMethod { get; init; }
    public IReadOnlyList<ItemInfo> Items { get; init; }

    public ReturnCreatedEvent(
        Guid returnId,
        string returnNumber,
        Guid originalSaleId,
        string originalTransactionNumber,
        string storeId,
        decimal refundAmount,
        string refundMethod,
        IReadOnlyList<ItemInfo> items)
    {
        ReturnId = returnId;
        ReturnNumber = returnNumber;
        OriginalSaleId = originalSaleId;
        OriginalTransactionNumber = originalTransactionNumber;
        StoreId = storeId;
        RefundAmount = refundAmount;
        RefundMethod = refundMethod;
        Items = items;
    }

    public sealed record ItemInfo(
        string Sku,
        int Quantity,
        string Condition,
        string? WarehouseId,
        string? LocationId,
        bool RestockRequired);
}
