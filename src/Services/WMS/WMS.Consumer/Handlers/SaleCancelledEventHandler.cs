using Common.Events.Models;
using Common.PostgreSQL.UnitOfWork;
using Microsoft.Extensions.Logging;
using WMS.Domain.Services;

namespace WMS.Consumer.Handlers;

/// <summary>
/// Handler for SaleCancelledEvent
/// </summary>
public sealed class SaleCancelledEventHandler
{
    private readonly InventoryService _inventoryService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SaleCancelledEventHandler> _logger;

    public SaleCancelledEventHandler(
        InventoryService inventoryService,
        IUnitOfWork unitOfWork,
        ILogger<SaleCancelledEventHandler> logger)
    {
        _inventoryService = inventoryService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task HandleAsync(SaleCancelledEvent @event, CancellationToken cancellationToken)
    {
        if (!@event.WasCompleted)
        {
            _logger.LogInformation(
                "Skipping SaleCancelledEvent for sale {SaleId} - sale was not completed, no inventory to restore",
                @event.SaleId);
            return;
        }

        _logger.LogInformation(
            "Processing SaleCancelledEvent. SaleId: {SaleId}, Reason: {Reason}, ItemCount: {ItemCount}",
            @event.SaleId, @event.CancellationReason, @event.Items.Count);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            foreach (var item in @event.Items)
            {
                var warehouseId = item.WarehouseId ?? "DEFAULT";

                await _inventoryService.RestoreStockForCancellationAsync(
                    item.Sku,
                    warehouseId,
                    item.Quantity,
                    @event.SaleId.ToString(),
                    @event.EventId.ToString(),
                    cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation(
                "Successfully processed SaleCancelledEvent for sale {SaleId}",
                @event.SaleId);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to process SaleCancelledEvent for sale {SaleId}",
                @event.SaleId);

            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
