using Microsoft.Extensions.Logging;
using WMS.Domain.Entities;
using WMS.Domain.Repositories;

namespace WMS.Domain.Services;

/// <summary>
/// Domain service for inventory operations
/// </summary>
public sealed class InventoryService
{
    private readonly IInventoryRepository _repository;
    private readonly ILogger<InventoryService> _logger;

    public InventoryService(IInventoryRepository repository, ILogger<InventoryService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task DeductStockForSaleAsync(
        string sku,
        string warehouseId,
        int quantity,
        string saleId,
        string? eventId = null,
        CancellationToken cancellationToken = default)
    {
        var inventory = await GetOrCreateInventoryAsync(sku, warehouseId, cancellationToken);

        _logger.LogInformation(
            "Deducting {Quantity} units of {Sku} from warehouse {WarehouseId} for sale {SaleId}",
            quantity, sku, warehouseId, saleId);

        var quantityBefore = inventory.QuantityOnHand;
        inventory.DeductStock(quantity, "Sale completed", saleId);

        var transaction = InventoryTransaction.Create(
            inventory,
            TransactionType.Deduction,
            quantity,
            quantityBefore,
            "Sale completed",
            saleId,
            "Sale",
            eventId);

        _repository.Update(inventory);
        await _repository.AddTransactionAsync(transaction, cancellationToken);

        if (inventory.IsLowStock)
        {
            _logger.LogWarning(
                "Low stock alert: {Sku} in warehouse {WarehouseId}. Available: {Available}, Reorder Point: {ReorderPoint}",
                sku, warehouseId, inventory.QuantityAvailable, inventory.ReorderPoint);
        }
    }

    public async Task RestoreStockForCancellationAsync(
        string sku,
        string warehouseId,
        int quantity,
        string saleId,
        string? eventId = null,
        CancellationToken cancellationToken = default)
    {
        var inventory = await GetOrCreateInventoryAsync(sku, warehouseId, cancellationToken);

        _logger.LogInformation(
            "Restoring {Quantity} units of {Sku} to warehouse {WarehouseId} for cancelled sale {SaleId}",
            quantity, sku, warehouseId, saleId);

        var quantityBefore = inventory.QuantityOnHand;
        inventory.AddStock(quantity, "Sale cancelled", saleId);

        var transaction = InventoryTransaction.Create(
            inventory,
            TransactionType.Addition,
            quantity,
            quantityBefore,
            "Sale cancelled - stock restored",
            saleId,
            "SaleCancellation",
            eventId);

        _repository.Update(inventory);
        await _repository.AddTransactionAsync(transaction, cancellationToken);
    }

    public async Task RestoreStockForReturnAsync(
        string sku,
        string warehouseId,
        int quantity,
        string returnId,
        string condition,
        bool restockRequired,
        string? eventId = null,
        CancellationToken cancellationToken = default)
    {
        if (!restockRequired)
        {
            _logger.LogInformation(
                "Skipping restock for return {ReturnId}, item {Sku} - restock not required (condition: {Condition})",
                returnId, sku, condition);
            return;
        }

        var inventory = await GetOrCreateInventoryAsync(sku, warehouseId, cancellationToken);

        _logger.LogInformation(
            "Restoring {Quantity} units of {Sku} to warehouse {WarehouseId} for return {ReturnId}",
            quantity, sku, warehouseId, returnId);

        var quantityBefore = inventory.QuantityOnHand;
        inventory.AddStock(quantity, $"Return - {condition}", returnId);

        var transaction = InventoryTransaction.Create(
            inventory,
            TransactionType.Addition,
            quantity,
            quantityBefore,
            $"Return processed - condition: {condition}",
            returnId,
            "Return",
            eventId);

        _repository.Update(inventory);
        await _repository.AddTransactionAsync(transaction, cancellationToken);
    }

    private async Task<Inventory> GetOrCreateInventoryAsync(
        string sku,
        string warehouseId,
        CancellationToken cancellationToken)
    {
        var inventory = await _repository.GetBySkuAndWarehouseAsync(sku, warehouseId, cancellationToken);

        if (inventory is null)
        {
            _logger.LogWarning(
                "Inventory not found for {Sku} in warehouse {WarehouseId}, creating new record",
                sku, warehouseId);

            inventory = Inventory.Create(sku, warehouseId, null);
            await _repository.AddAsync(inventory, cancellationToken);
        }

        return inventory;
    }
}
