using MediatR;

namespace POS.Application.Commands.Sales;

/// <summary>
/// Command to create a new sale
/// </summary>
public sealed record CreateSaleCommand : IRequest<CreateSaleResult>
{
    public required string StoreId { get; init; }
    public required string TerminalId { get; init; }
    public string? CashierId { get; init; }
    public string? CustomerId { get; init; }
    public string Currency { get; init; } = "IDR";
    public required IReadOnlyList<SaleItemRequest> Items { get; init; }
}

public sealed record SaleItemRequest
{
    public required string Sku { get; init; }
    public required string ProductName { get; init; }
    public required int Quantity { get; init; }
    public required decimal UnitPrice { get; init; }
    public decimal TaxRate { get; init; } = 0.11m;
    public decimal DiscountAmount { get; init; } = 0;
    public string? WarehouseId { get; init; }
    public string? LocationId { get; init; }
}

public sealed record CreateSaleResult
{
    public required Guid SaleId { get; init; }
    public required string TransactionNumber { get; init; }
    public required decimal TotalAmount { get; init; }
}
