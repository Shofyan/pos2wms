using WMS.Domain.Entities;

namespace WMS.Domain.Repositories;

/// <summary>
/// Repository interface for Inventory
/// </summary>
public interface IInventoryRepository
{
    Task<Inventory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Inventory?> GetBySkuAndWarehouseAsync(
        string sku,
        string warehouseId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Inventory>> GetBySkuAsync(
        string sku,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Inventory>> GetByWarehouseAsync(
        string warehouseId,
        int pageNumber = 1,
        int pageSize = 100,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Inventory>> GetLowStockAsync(
        string? warehouseId = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(Inventory inventory, CancellationToken cancellationToken = default);

    void Update(Inventory inventory);

    Task AddTransactionAsync(InventoryTransaction transaction, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InventoryTransaction>> GetTransactionsBySkuAsync(
        string sku,
        DateTimeOffset from,
        DateTimeOffset to,
        CancellationToken cancellationToken = default);
}
