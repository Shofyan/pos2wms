using Common.Events.Abstractions;

namespace Common.Events.Models;

/// <summary>
/// Event raised when a sale is cancelled
/// </summary>
public sealed record SaleCancelledEvent : IntegrationEvent
{
    public override string EventType => "pos.sale.cancelled";

    public required Guid SaleId { get; init; }
    public required string TransactionNumber { get; init; }
    public required string StoreId { get; init; }
    public required string TerminalId { get; init; }
    public required string? CashierId { get; init; }
    public required DateTimeOffset CancelledAt { get; init; }
    public required string CancellationReason { get; init; }
    public required string? AuthorizedBy { get; init; }
    public required bool WasCompleted { get; init; }
    public required IReadOnlyList<CancelledItemEvent> Items { get; init; }
}

/// <summary>
/// Cancelled item details for inventory restoration
/// </summary>
public sealed record CancelledItemEvent
{
    public required Guid SaleItemId { get; init; }
    public required string Sku { get; init; }
    public required int Quantity { get; init; }
    public required string? WarehouseId { get; init; }
    public required string? LocationId { get; init; }
}
