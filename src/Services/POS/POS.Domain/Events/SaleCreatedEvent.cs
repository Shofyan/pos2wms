using POS.Domain.Common;

namespace POS.Domain.Events;

/// <summary>
/// Event raised when a sale is created
/// </summary>
public sealed record SaleCreatedEvent(
    Guid SaleId,
    string TransactionNumber,
    string StoreId) : DomainEvent
{
    public override string EventType => "pos.sale.created";
}
