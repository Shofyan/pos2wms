using POS.Domain.Common;
using POS.Domain.ValueObjects;

namespace POS.Domain.Entities;

/// <summary>
/// Sale item entity representing a product in a sale
/// </summary>
public sealed class SaleItem : Entity
{
    public Guid SaleId { get; private set; }
    public Sale Sale { get; private set; } = null!;
    public SKU Sku { get; private set; } = null!;
    public string ProductName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; } = null!;
    public decimal TaxRate { get; private set; }
    public Money TaxAmount { get; private set; } = null!;
    public Money DiscountAmount { get; private set; } = null!;
    public Money TotalPrice { get; private set; } = null!;
    public string? WarehouseId { get; private set; }
    public string? LocationId { get; private set; }

    private SaleItem() { } // EF Core

    internal static SaleItem Create(
        Sale sale,
        SKU sku,
        string productName,
        int quantity,
        Money unitPrice,
        decimal taxRate,
        Money discountAmount,
        string? warehouseId,
        string? locationId)
    {
        var item = new SaleItem
        {
            Id = Guid.NewGuid(),
            SaleId = sale.Id,
            Sale = sale,
            Sku = sku,
            ProductName = productName,
            Quantity = quantity,
            UnitPrice = unitPrice,
            TaxRate = taxRate,
            DiscountAmount = discountAmount,
            WarehouseId = warehouseId,
            LocationId = locationId
        };

        item.RecalculateAmounts();
        return item;
    }

    internal void IncreaseQuantity(int additionalQuantity)
    {
        Quantity += additionalQuantity;
        RecalculateAmounts();
    }

    internal void SetQuantity(int newQuantity)
    {
        Quantity = newQuantity;
        RecalculateAmounts();
    }

    internal void ApplyDiscount(Money discount)
    {
        DiscountAmount = discount;
        RecalculateAmounts();
    }

    private void RecalculateAmounts()
    {
        var lineTotal = UnitPrice.Multiply(Quantity);
        var taxableAmount = lineTotal.Subtract(DiscountAmount);
        TaxAmount = taxableAmount.Multiply(TaxRate);
        TotalPrice = lineTotal.Add(TaxAmount).Subtract(DiscountAmount);
    }
}
