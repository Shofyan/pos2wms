using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Domain.Repositories;
using WMS.Infrastructure.Data;

namespace WMS.Infrastructure.Repositories;

/// <summary>
/// Inventory repository implementation
/// </summary>
public sealed class InventoryRepository : IInventoryRepository
{
    private readonly WmsDbContext _context;

    public InventoryRepository(WmsDbContext context)
    {
        _context = context;
    }

    public async Task<Inventory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Inventories
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<Inventory?> GetBySkuAndWarehouseAsync(
        string sku,
        string warehouseId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Inventories
            .FirstOrDefaultAsync(i =>
                i.Sku == sku.ToUpperInvariant() &&
                i.WarehouseId == warehouseId,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Inventory>> GetBySkuAsync(
        string sku,
        CancellationToken cancellationToken = default)
    {
        return await _context.Inventories
            .Where(i => i.Sku == sku.ToUpperInvariant())
            .OrderBy(i => i.WarehouseId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Inventory>> GetByWarehouseAsync(
        string warehouseId,
        int pageNumber = 1,
        int pageSize = 100,
        CancellationToken cancellationToken = default)
    {
        return await _context.Inventories
            .Where(i => i.WarehouseId == warehouseId)
            .OrderBy(i => i.Sku)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Inventory>> GetLowStockAsync(
        string? warehouseId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Inventories.AsQueryable();

        if (!string.IsNullOrEmpty(warehouseId))
        {
            query = query.Where(i => i.WarehouseId == warehouseId);
        }

        return await query
            .Where(i => i.QuantityOnHand - i.QuantityReserved <= i.ReorderPoint)
            .OrderBy(i => i.QuantityOnHand - i.QuantityReserved)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Inventory inventory, CancellationToken cancellationToken = default)
    {
        await _context.Inventories.AddAsync(inventory, cancellationToken);
    }

    public void Update(Inventory inventory)
    {
        _context.Inventories.Update(inventory);
    }

    public async Task AddTransactionAsync(InventoryTransaction transaction, CancellationToken cancellationToken = default)
    {
        await _context.InventoryTransactions.AddAsync(transaction, cancellationToken);
    }

    public async Task<IReadOnlyList<InventoryTransaction>> GetTransactionsBySkuAsync(
        string sku,
        DateTimeOffset from,
        DateTimeOffset to,
        CancellationToken cancellationToken = default)
    {
        return await _context.InventoryTransactions
            .Where(t => t.Sku == sku.ToUpperInvariant())
            .Where(t => t.TransactionDate >= from && t.TransactionDate <= to)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync(cancellationToken);
    }
}
