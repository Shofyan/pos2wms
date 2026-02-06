using MediatR;

namespace POS.Application.Commands.Returns;

/// <summary>
/// Command to create a return
/// </summary>
public sealed record CreateReturnCommand : IRequest<CreateReturnResult>
{
    public required Guid OriginalSaleId { get; init; }
    public required string StoreId { get; init; }
    public required string TerminalId { get; init; }
    public string? CashierId { get; init; }
    public required string ReturnReason { get; init; }
    public required string RefundMethod { get; init; }
    public required IReadOnlyList<ReturnItemRequest> Items { get; init; }
}

public sealed record ReturnItemRequest
{
    public required Guid OriginalSaleItemId { get; init; }
    public required int Quantity { get; init; }
    public required string Condition { get; init; }
    public bool RestockRequired { get; init; } = true;
}

public sealed record CreateReturnResult
{
    public required Guid ReturnId { get; init; }
    public required string ReturnNumber { get; init; }
    public required decimal RefundAmount { get; init; }
}
