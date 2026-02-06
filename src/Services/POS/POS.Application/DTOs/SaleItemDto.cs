namespace POS.Application.DTOs;

/// <summary>
/// Sale item data transfer object
/// </summary>
public sealed record SaleItemDto
{
    public required Guid Id { get; init; }
    public required string Sku { get; init; }
    public required string ProductName { get; init; }
    public required int Quantity { get; init; }
    public required decimal UnitPrice { get; init; }
    public required decimal TaxRate { get; init; }
    public required decimal TaxAmount { get; init; }
    public required decimal DiscountAmount { get; init; }
    public required decimal TotalPrice { get; init; }
    public string? WarehouseId { get; init; }
    public string? LocationId { get; init; }
}
