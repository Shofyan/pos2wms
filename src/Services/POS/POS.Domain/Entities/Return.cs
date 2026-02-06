using Common.PostgreSQL.Interceptors;
using POS.Domain.Common;
using POS.Domain.Events;
using POS.Domain.Exceptions;
using POS.Domain.ValueObjects;

namespace POS.Domain.Entities;

/// <summary>
/// Return aggregate root representing a product return
/// </summary>
public sealed class Return : AggregateRoot, IAuditableEntity
{
    private readonly List<ReturnItem> _items = new();

    public string ReturnNumber { get; private set; } = string.Empty;
    public Guid OriginalSaleId { get; private set; }
    public string OriginalTransactionNumber { get; private set; } = string.Empty;
    public StoreId StoreId { get; private set; } = null!;
    public TerminalId TerminalId { get; private set; } = null!;
    public string? CashierId { get; private set; }
    public string? CustomerId { get; private set; }
    public ReturnStatus Status { get; private set; }
    public string ReturnReason { get; private set; } = string.Empty;
    public Money RefundAmount { get; private set; } = Money.Zero();
    public string RefundMethod { get; private set; } = string.Empty;
    public DateTimeOffset? ProcessedAt { get; private set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public IReadOnlyList<ReturnItem> Items => _items.AsReadOnly();

    private Return() { } // EF Core

    public static Return Create(
        Sale originalSale,
        StoreId storeId,
        TerminalId terminalId,
        string? cashierId,
        string returnReason,
        string refundMethod)
    {
        if (originalSale.Status != SaleStatus.Completed)
            throw new InvalidReturnException("Can only create return for completed sales");

        if (string.IsNullOrWhiteSpace(returnReason))
            throw new InvalidReturnException("Return reason is required");

        var returnEntity = new Return
        {
            Id = Guid.NewGuid(),
            ReturnNumber = GenerateReturnNumber(storeId),
            OriginalSaleId = originalSale.Id,
            OriginalTransactionNumber = originalSale.TransactionNumber,
            StoreId = storeId,
            TerminalId = terminalId,
            CashierId = cashierId,
            CustomerId = originalSale.CustomerId,
            Status = ReturnStatus.Pending,
            ReturnReason = returnReason,
            RefundMethod = refundMethod,
            RefundAmount = Money.Zero(originalSale.TotalAmount.Currency)
        };

        return returnEntity;
    }

    public void AddItem(
        SaleItem originalItem,
        int quantity,
        string condition,
        bool restockRequired = true)
    {
        EnsureCanModify();

        if (quantity <= 0)
            throw new InvalidReturnException("Quantity must be greater than zero");

        if (quantity > originalItem.Quantity)
            throw new InvalidReturnException("Return quantity cannot exceed original quantity");

        var existingItem = _items.FirstOrDefault(i => i.OriginalSaleItemId == originalItem.Id);
        if (existingItem is not null)
        {
            var totalQuantity = existingItem.Quantity + quantity;
            if (totalQuantity > originalItem.Quantity)
                throw new InvalidReturnException("Total return quantity cannot exceed original quantity");

            existingItem.IncreaseQuantity(quantity);
        }
        else
        {
            var returnItem = ReturnItem.Create(this, originalItem, quantity, condition, restockRequired);
            _items.Add(returnItem);
        }

        RecalculateRefundAmount();
        IncrementVersion();
    }

    public void Process()
    {
        if (Status != ReturnStatus.Pending)
            throw new InvalidReturnException("Return is not pending");

        if (!_items.Any())
            throw new InvalidReturnException("Cannot process empty return");

        Status = ReturnStatus.Processed;
        ProcessedAt = DateTimeOffset.UtcNow;
        IncrementVersion();

        AddDomainEvent(new ReturnCreatedEvent(
            Id,
            ReturnNumber,
            OriginalSaleId,
            OriginalTransactionNumber,
            StoreId.Value,
            RefundAmount.Amount,
            RefundMethod,
            _items.Select(i => new ReturnCreatedEvent.ItemInfo(
                i.Sku.Value,
                i.Quantity,
                i.Condition,
                i.WarehouseId,
                i.LocationId,
                i.RestockRequired)).ToList()));
    }

    public void Cancel(string reason)
    {
        if (Status == ReturnStatus.Cancelled)
            throw new InvalidReturnException("Return is already cancelled");

        if (Status == ReturnStatus.Processed)
            throw new InvalidReturnException("Cannot cancel processed return");

        Status = ReturnStatus.Cancelled;
        IncrementVersion();
    }

    private void RecalculateRefundAmount()
    {
        RefundAmount = _items.Aggregate(
            Money.Zero(RefundAmount.Currency),
            (total, item) => total.Add(item.RefundAmount));
    }

    private void EnsureCanModify()
    {
        if (Status != ReturnStatus.Pending)
            throw new InvalidReturnException("Cannot modify non-pending return");
    }

    private static string GenerateReturnNumber(StoreId storeId)
    {
        var date = DateTime.UtcNow;
        var random = Random.Shared.Next(1000, 9999);
        return $"RTN-{storeId.Value}-{date:yyyyMMddHHmmss}-{random}";
    }
}

/// <summary>
/// Return status enumeration
/// </summary>
public enum ReturnStatus
{
    Pending = 0,
    Processed = 1,
    Cancelled = 2
}
