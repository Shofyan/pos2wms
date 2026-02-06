namespace WMS.Domain.Entities;

/// <summary>
/// Inventory transaction for audit trail
/// </summary>
public sealed class InventoryTransaction
{
    public Guid Id { get; private set; }
    public Guid InventoryId { get; private set; }
    public string Sku { get; private set; } = string.Empty;
    public string WarehouseId { get; private set; } = string.Empty;
    public TransactionType Type { get; private set; }
    public int Quantity { get; private set; }
    public int QuantityBefore { get; private set; }
    public int QuantityAfter { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public string ReferenceId { get; private set; } = string.Empty;
    public string ReferenceType { get; private set; } = string.Empty;
    public string? SourceEventId { get; private set; }
    public DateTimeOffset TransactionDate { get; private set; }
    public string? CreatedBy { get; private set; }

    private InventoryTransaction() { } // EF Core

    public static InventoryTransaction Create(
        Inventory inventory,
        TransactionType type,
        int quantity,
        int quantityBefore,
        string reason,
        string referenceId,
        string referenceType,
        string? sourceEventId = null,
        string? createdBy = null)
    {
        return new InventoryTransaction
        {
            Id = Guid.NewGuid(),
            InventoryId = inventory.Id,
            Sku = inventory.Sku,
            WarehouseId = inventory.WarehouseId,
            Type = type,
            Quantity = quantity,
            QuantityBefore = quantityBefore,
            QuantityAfter = type == TransactionType.Deduction
                ? quantityBefore - quantity
                : quantityBefore + quantity,
            Reason = reason,
            ReferenceId = referenceId,
            ReferenceType = referenceType,
            SourceEventId = sourceEventId,
            TransactionDate = DateTimeOffset.UtcNow,
            CreatedBy = createdBy
        };
    }
}

public enum TransactionType
{
    Addition = 1,
    Deduction = 2,
    Reservation = 3,
    ReservationRelease = 4,
    Adjustment = 5,
    Transfer = 6
}
