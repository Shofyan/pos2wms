using MediatR;

namespace POS.Application.Commands.Sales;

/// <summary>
/// Command to complete a sale with payment
/// </summary>
public sealed record CompleteSaleCommand : IRequest<CompleteSaleResult>
{
    public required Guid SaleId { get; init; }
    public required IReadOnlyList<PaymentRequest> Payments { get; init; }
}

public sealed record PaymentRequest
{
    public required string PaymentMethod { get; init; }
    public required decimal Amount { get; init; }
    public string? Reference { get; init; }
}

public sealed record CompleteSaleResult
{
    public required Guid SaleId { get; init; }
    public required string TransactionNumber { get; init; }
    public required decimal TotalAmount { get; init; }
    public required decimal PaidAmount { get; init; }
    public required decimal ChangeAmount { get; init; }
    public required DateTimeOffset CompletedAt { get; init; }
}
