namespace POS.Application.DTOs;

/// <summary>
/// Sale data transfer object
/// </summary>
public sealed record SaleDto
{
    public required Guid Id { get; init; }
    public required string TransactionNumber { get; init; }
    public required string StoreId { get; init; }
    public required string TerminalId { get; init; }
    public string? CashierId { get; init; }
    public string? CustomerId { get; init; }
    public required string Status { get; init; }
    public required decimal SubTotal { get; init; }
    public required decimal TaxAmount { get; init; }
    public required decimal DiscountAmount { get; init; }
    public required decimal TotalAmount { get; init; }
    public required decimal PaidAmount { get; init; }
    public required decimal ChangeAmount { get; init; }
    public required string Currency { get; init; }
    public DateTimeOffset? CompletedAt { get; init; }
    public DateTimeOffset? CancelledAt { get; init; }
    public string? CancellationReason { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required DateTimeOffset UpdatedAt { get; init; }
    public required IReadOnlyList<SaleItemDto> Items { get; init; }
    public required IReadOnlyList<PaymentDto> Payments { get; init; }
}
