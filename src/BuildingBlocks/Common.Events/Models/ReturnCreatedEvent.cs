using Common.Events.Abstractions;

namespace Common.Events.Models;

/// <summary>
/// Event raised when a return is created
/// </summary>
public sealed record ReturnCreatedEvent : IntegrationEvent
{
    public override string EventType => "pos.return.created";

    public required Guid ReturnId { get; init; }
    public required string ReturnNumber { get; init; }
    public required Guid OriginalSaleId { get; init; }
    public required string OriginalTransactionNumber { get; init; }
    public required string StoreId { get; init; }
    public required string TerminalId { get; init; }
    public required string? CashierId { get; init; }
    public required string? CustomerId { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required string ReturnReason { get; init; }
    public required decimal RefundAmount { get; init; }
    public required string RefundMethod { get; init; }
    public required string Currency { get; init; }
    public required IReadOnlyList<ReturnItemEvent> Items { get; init; }
}

/// <summary>
/// Return item details for inventory restoration
/// </summary>
public sealed record ReturnItemEvent
{
    public required Guid ReturnItemId { get; init; }
    public required Guid OriginalSaleItemId { get; init; }
    public required string Sku { get; init; }
    public required string ProductName { get; init; }
    public required int Quantity { get; init; }
    public required decimal RefundAmount { get; init; }
    public required string Condition { get; init; }
    public required string? WarehouseId { get; init; }
    public required string? LocationId { get; init; }
    public required bool RestockRequired { get; init; }
}
