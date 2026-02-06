using Common.PostgreSQL.Interceptors;

namespace WMS.Domain.Entities;

/// <summary>
/// Inventory entity representing stock for a product
/// </summary>
public sealed class Inventory : IAuditableEntity
{
    public Guid Id { get; private set; }
    public string Sku { get; private set; } = string.Empty;
    public string WarehouseId { get; private set; } = string.Empty;
    public string? LocationId { get; private set; }
    public int QuantityOnHand { get; private set; }
    public int QuantityReserved { get; private set; }
    public int QuantityAvailable => QuantityOnHand - QuantityReserved;
    public int ReorderPoint { get; private set; }
    public int ReorderQuantity { get; private set; }
    public DateTimeOffset LastStockCheck { get; private set; }
    public int Version { get; private set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    private Inventory() { } // EF Core

    public static Inventory Create(
        string sku,
        string warehouseId,
        string? locationId,
        int initialQuantity = 0,
        int reorderPoint = 10,
        int reorderQuantity = 50)
    {
        return new Inventory
        {
            Id = Guid.NewGuid(),
            Sku = sku.ToUpperInvariant(),
            WarehouseId = warehouseId,
            LocationId = locationId,
            QuantityOnHand = initialQuantity,
            QuantityReserved = 0,
            ReorderPoint = reorderPoint,
            ReorderQuantity = reorderQuantity,
            LastStockCheck = DateTimeOffset.UtcNow
        };
    }

    public void DeductStock(int quantity, string reason, string referenceId)
    {
        if (quantity <= 0)
            throw new InvalidOperationException("Quantity must be positive");

        if (quantity > QuantityAvailable)
            throw new InvalidOperationException($"Insufficient stock. Available: {QuantityAvailable}, Requested: {quantity}");

        QuantityOnHand -= quantity;
        Version++;
    }

    public void AddStock(int quantity, string reason, string referenceId)
    {
        if (quantity <= 0)
            throw new InvalidOperationException("Quantity must be positive");

        QuantityOnHand += quantity;
        Version++;
    }

    public void Reserve(int quantity, string referenceId)
    {
        if (quantity <= 0)
            throw new InvalidOperationException("Quantity must be positive");

        if (quantity > QuantityAvailable)
            throw new InvalidOperationException($"Insufficient available stock. Available: {QuantityAvailable}, Requested: {quantity}");

        QuantityReserved += quantity;
        Version++;
    }

    public void ReleaseReservation(int quantity, string referenceId)
    {
        if (quantity <= 0)
            throw new InvalidOperationException("Quantity must be positive");

        if (quantity > QuantityReserved)
            quantity = QuantityReserved;

        QuantityReserved -= quantity;
        Version++;
    }

    public void AdjustStock(int newQuantity, string reason, string adjustedBy)
    {
        if (newQuantity < 0)
            throw new InvalidOperationException("Quantity cannot be negative");

        QuantityOnHand = newQuantity;
        LastStockCheck = DateTimeOffset.UtcNow;
        Version++;
    }

    public bool IsLowStock => QuantityAvailable <= ReorderPoint;
    public bool IsOutOfStock => QuantityAvailable <= 0;
}
