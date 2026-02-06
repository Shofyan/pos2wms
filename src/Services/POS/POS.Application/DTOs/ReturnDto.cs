namespace POS.Application.DTOs;

/// <summary>
/// Return data transfer object
/// </summary>
public sealed record ReturnDto
{
    public required Guid Id { get; init; }
    public required string ReturnNumber { get; init; }
    public required Guid OriginalSaleId { get; init; }
    public required string OriginalTransactionNumber { get; init; }
    public required string StoreId { get; init; }
    public required string TerminalId { get; init; }
    public string? CashierId { get; init; }
    public string? CustomerId { get; init; }
    public required string Status { get; init; }
    public required string ReturnReason { get; init; }
    public required decimal RefundAmount { get; init; }
    public required string RefundMethod { get; init; }
    public required string Currency { get; init; }
    public DateTimeOffset? ProcessedAt { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required DateTimeOffset UpdatedAt { get; init; }
    public required IReadOnlyList<ReturnItemDto> Items { get; init; }
}

/// <summary>
/// Return item data transfer object
/// </summary>
public sealed record ReturnItemDto
{
    public required Guid Id { get; init; }
    public required Guid OriginalSaleItemId { get; init; }
    public required string Sku { get; init; }
    public required string ProductName { get; init; }
    public required int Quantity { get; init; }
    public required decimal RefundAmount { get; init; }
    public required string Condition { get; init; }
    public string? WarehouseId { get; init; }
    public string? LocationId { get; init; }
    public required bool RestockRequired { get; init; }
}
