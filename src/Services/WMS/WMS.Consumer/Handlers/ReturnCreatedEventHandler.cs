using Common.Events.Models;
using Common.PostgreSQL.UnitOfWork;
using Microsoft.Extensions.Logging;
using WMS.Domain.Services;

namespace WMS.Consumer.Handlers;

/// <summary>
/// Handler for ReturnCreatedEvent
/// </summary>
public sealed class ReturnCreatedEventHandler
{
    private readonly InventoryService _inventoryService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ReturnCreatedEventHandler> _logger;

    public ReturnCreatedEventHandler(
        InventoryService inventoryService,
        IUnitOfWork unitOfWork,
        ILogger<ReturnCreatedEventHandler> logger)
    {
        _inventoryService = inventoryService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task HandleAsync(ReturnCreatedEvent @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Processing ReturnCreatedEvent. ReturnId: {ReturnId}, ReturnNumber: {ReturnNumber}, ItemCount: {ItemCount}",
            @event.ReturnId, @event.ReturnNumber, @event.Items.Count);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            foreach (var item in @event.Items)
            {
                var warehouseId = item.WarehouseId ?? "DEFAULT";

                await _inventoryService.RestoreStockForReturnAsync(
                    item.Sku,
                    warehouseId,
                    item.Quantity,
                    @event.ReturnId.ToString(),
                    item.Condition,
                    item.RestockRequired,
                    @event.EventId.ToString(),
                    cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation(
                "Successfully processed ReturnCreatedEvent for return {ReturnId}",
                @event.ReturnId);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to process ReturnCreatedEvent for return {ReturnId}",
                @event.ReturnId);

            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
