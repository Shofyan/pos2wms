using POS.Domain.Common;
using POS.Domain.ValueObjects;

namespace POS.Domain.Entities;

/// <summary>
/// Return item entity
/// </summary>
public sealed class ReturnItem : Entity
{
    public Guid ReturnId { get; private set; }
    public Return Return { get; private set; } = null!;
    public Guid OriginalSaleItemId { get; private set; }
    public SKU Sku { get; private set; } = null!;
    public string ProductName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public Money RefundAmount { get; private set; } = null!;
    public string Condition { get; private set; } = string.Empty;
    public string? WarehouseId { get; private set; }
    public string? LocationId { get; private set; }
    public bool RestockRequired { get; private set; }

    private ReturnItem() { } // EF Core

    internal static ReturnItem Create(
        Return returnEntity,
        SaleItem originalItem,
        int quantity,
        string condition,
        bool restockRequired)
    {
        var refundPerUnit = originalItem.TotalPrice.Amount / originalItem.Quantity;

        return new ReturnItem
        {
            Id = Guid.NewGuid(),
            ReturnId = returnEntity.Id,
            Return = returnEntity,
            OriginalSaleItemId = originalItem.Id,
            Sku = originalItem.Sku,
            ProductName = originalItem.ProductName,
            Quantity = quantity,
            RefundAmount = Money.Create(refundPerUnit * quantity, originalItem.UnitPrice.Currency),
            Condition = condition,
            WarehouseId = originalItem.WarehouseId,
            LocationId = originalItem.LocationId,
            RestockRequired = restockRequired
        };
    }

    internal void IncreaseQuantity(int additionalQuantity)
    {
        var refundPerUnit = RefundAmount.Amount / Quantity;
        Quantity += additionalQuantity;
        RefundAmount = Money.Create(refundPerUnit * Quantity, RefundAmount.Currency);
    }
}
