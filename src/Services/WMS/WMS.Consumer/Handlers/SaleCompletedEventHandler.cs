using Common.Events.Models;
using Common.Kafka.Abstractions;
using Common.PostgreSQL.UnitOfWork;
using Microsoft.Extensions.Logging;
using WMS.Domain.Services;

namespace WMS.Consumer.Handlers;

/// <summary>
/// Handler for SaleCompletedEvent
/// </summary>
public sealed class SaleCompletedEventHandler
{
    private readonly InventoryService _inventoryService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SaleCompletedEventHandler> _logger;

    public SaleCompletedEventHandler(
        InventoryService inventoryService,
        IUnitOfWork unitOfWork,
        ILogger<SaleCompletedEventHandler> logger)
    {
        _inventoryService = inventoryService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task HandleAsync(SaleCompletedEvent @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Processing SaleCompletedEvent. SaleId: {SaleId}, TransactionNumber: {TransactionNumber}, ItemCount: {ItemCount}",
            @event.SaleId, @event.TransactionNumber, @event.Items.Count);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            foreach (var item in @event.Items)
            {
                var warehouseId = item.WarehouseId ?? "DEFAULT";

                await _inventoryService.DeductStockForSaleAsync(
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
                "Successfully processed SaleCompletedEvent for sale {SaleId}",
                @event.SaleId);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to process SaleCompletedEvent for sale {SaleId}",
                @event.SaleId);

            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
