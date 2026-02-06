using Common.Events.Abstractions;

namespace Common.Events.Models;

/// <summary>
/// Event raised when a sale is completed
/// </summary>
public sealed record SaleCompletedEvent : IntegrationEvent
{
    public override string EventType => "pos.sale.completed";

    public required Guid SaleId { get; init; }
    public required string TransactionNumber { get; init; }
    public required string StoreId { get; init; }
    public required string TerminalId { get; init; }
    public required string? CashierId { get; init; }
    public required string? CustomerId { get; init; }
    public required DateTimeOffset CompletedAt { get; init; }
    public required decimal TotalAmount { get; init; }
    public required decimal TaxAmount { get; init; }
    public required decimal DiscountAmount { get; init; }
    public required string Currency { get; init; }
    public required IReadOnlyList<SaleItemEvent> Items { get; init; }
    public required IReadOnlyList<PaymentEvent> Payments { get; init; }
}

/// <summary>
/// Sale item details within an event
/// </summary>
public sealed record SaleItemEvent
{
    public required Guid SaleItemId { get; init; }
    public required string Sku { get; init; }
    public required string ProductName { get; init; }
    public required int Quantity { get; init; }
    public required decimal UnitPrice { get; init; }
    public required decimal DiscountAmount { get; init; }
    public required decimal TaxAmount { get; init; }
    public required decimal TotalPrice { get; init; }
    public required string? WarehouseId { get; init; }
    public required string? LocationId { get; init; }
}

/// <summary>
/// Payment details within an event
/// </summary>
public sealed record PaymentEvent
{
    public required Guid PaymentId { get; init; }
    public required string PaymentMethod { get; init; }
    public required decimal Amount { get; init; }
    public required string Currency { get; init; }
    public required string? Reference { get; init; }
    public required DateTimeOffset ProcessedAt { get; init; }
}
