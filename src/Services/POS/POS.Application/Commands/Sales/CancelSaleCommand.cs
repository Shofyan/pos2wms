using MediatR;

namespace POS.Application.Commands.Sales;

/// <summary>
/// Command to cancel a sale
/// </summary>
public sealed record CancelSaleCommand : IRequest<CancelSaleResult>
{
    public required Guid SaleId { get; init; }
    public required string Reason { get; init; }
    public string? AuthorizedBy { get; init; }
}

public sealed record CancelSaleResult
{
    public required Guid SaleId { get; init; }
    public required string TransactionNumber { get; init; }
    public required DateTimeOffset CancelledAt { get; init; }
    public required bool WasCompleted { get; init; }
}
