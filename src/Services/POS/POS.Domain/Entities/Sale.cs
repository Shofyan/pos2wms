using Common.PostgreSQL.Interceptors;
using POS.Domain.Common;
using POS.Domain.Events;
using POS.Domain.Exceptions;
using POS.Domain.ValueObjects;

namespace POS.Domain.Entities;

/// <summary>
/// Sale aggregate root representing a POS transaction
/// </summary>
public sealed class Sale : AggregateRoot, IAuditableEntity
{
    private readonly List<SaleItem> _items = new();
    private readonly List<Payment> _payments = new();

    public string TransactionNumber { get; private set; } = string.Empty;
    public StoreId StoreId { get; private set; } = null!;
    public TerminalId TerminalId { get; private set; } = null!;
    public string? CashierId { get; private set; }
    public string? CustomerId { get; private set; }
    public SaleStatus Status { get; private set; }
    public Money SubTotal { get; private set; } = Money.Zero();
    public Money TaxAmount { get; private set; } = Money.Zero();
    public Money DiscountAmount { get; private set; } = Money.Zero();
    public Money TotalAmount { get; private set; } = Money.Zero();
    public Money PaidAmount { get; private set; } = Money.Zero();
    public Money ChangeAmount { get; private set; } = Money.Zero();
    public DateTimeOffset? CompletedAt { get; private set; }
    public DateTimeOffset? CancelledAt { get; private set; }
    public string? CancellationReason { get; private set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public IReadOnlyList<SaleItem> Items => _items.AsReadOnly();
    public IReadOnlyList<Payment> Payments => _payments.AsReadOnly();

    private Sale() { } // EF Core

    public static Sale Create(
        StoreId storeId,
        TerminalId terminalId,
        string? cashierId,
        string? customerId,
        string currency = "IDR")
    {
        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            TransactionNumber = GenerateTransactionNumber(storeId, terminalId),
            StoreId = storeId,
            TerminalId = terminalId,
            CashierId = cashierId,
            CustomerId = customerId,
            Status = SaleStatus.Draft,
            SubTotal = Money.Zero(currency),
            TaxAmount = Money.Zero(currency),
            DiscountAmount = Money.Zero(currency),
            TotalAmount = Money.Zero(currency),
            PaidAmount = Money.Zero(currency),
            ChangeAmount = Money.Zero(currency)
        };

        sale.AddDomainEvent(new SaleCreatedEvent(sale.Id, sale.TransactionNumber, storeId.Value));
        return sale;
    }

    public void AddItem(
        string sku,
        string productName,
        int quantity,
        Money unitPrice,
        decimal taxRate = 0.11m,
        Money? discount = null,
        string? warehouseId = null,
        string? locationId = null)
    {
        EnsureCanModify();

        if (quantity <= 0)
            throw new InvalidSaleException("Quantity must be greater than zero");

        var existingItem = _items.FirstOrDefault(i => i.Sku.Value == sku.ToUpperInvariant());
        if (existingItem is not null)
        {
            existingItem.IncreaseQuantity(quantity);
        }
        else
        {
            var item = SaleItem.Create(
                this,
                SKU.Create(sku),
                productName,
                quantity,
                unitPrice,
                taxRate,
                discount ?? Money.Zero(unitPrice.Currency),
                warehouseId,
                locationId);

            _items.Add(item);
        }

        RecalculateTotals();
        IncrementVersion();
    }

    public void RemoveItem(Guid itemId)
    {
        EnsureCanModify();

        var item = _items.FirstOrDefault(i => i.Id == itemId)
            ?? throw new InvalidSaleException($"Item {itemId} not found");

        _items.Remove(item);
        RecalculateTotals();
        IncrementVersion();
    }

    public void UpdateItemQuantity(Guid itemId, int newQuantity)
    {
        EnsureCanModify();

        if (newQuantity <= 0)
        {
            RemoveItem(itemId);
            return;
        }

        var item = _items.FirstOrDefault(i => i.Id == itemId)
            ?? throw new InvalidSaleException($"Item {itemId} not found");

        item.SetQuantity(newQuantity);
        RecalculateTotals();
        IncrementVersion();
    }

    public void ApplyDiscount(Money discount)
    {
        EnsureCanModify();

        if (discount.IsNegative)
            throw new InvalidSaleException("Discount cannot be negative");

        if (discount.Amount > SubTotal.Amount)
            throw new InvalidSaleException("Discount cannot exceed subtotal");

        DiscountAmount = discount;
        RecalculateTotals();
        IncrementVersion();
    }

    public void AddPayment(string paymentMethod, Money amount, string? reference = null)
    {
        if (Status == SaleStatus.Completed)
            throw new InvalidSaleException("Sale is already completed");

        if (Status == SaleStatus.Cancelled)
            throw new InvalidSaleException("Cannot add payment to cancelled sale");

        if (!_items.Any())
            throw new InvalidSaleException("Cannot add payment to empty sale");

        var payment = Payment.Create(this, paymentMethod, amount, reference);
        _payments.Add(payment);

        PaidAmount = PaidAmount.Add(amount);

        if (PaidAmount.Amount >= TotalAmount.Amount)
        {
            ChangeAmount = PaidAmount.Subtract(TotalAmount);
            Status = SaleStatus.PendingCompletion;
        }

        IncrementVersion();
    }

    public void Complete()
    {
        if (Status == SaleStatus.Completed)
            throw new InvalidSaleException("Sale is already completed");

        if (Status == SaleStatus.Cancelled)
            throw new InvalidSaleException("Cannot complete cancelled sale");

        if (!_items.Any())
            throw new InvalidSaleException("Cannot complete empty sale");

        if (PaidAmount.Amount < TotalAmount.Amount)
            throw new InvalidSaleException("Insufficient payment amount");

        Status = SaleStatus.Completed;
        CompletedAt = DateTimeOffset.UtcNow;
        IncrementVersion();

        AddDomainEvent(new SaleCompletedEvent(
            Id,
            TransactionNumber,
            StoreId.Value,
            TotalAmount.Amount,
            _items.Select(i => new SaleCompletedEvent.ItemInfo(i.Sku.Value, i.Quantity, i.WarehouseId, i.LocationId)).ToList()));
    }

    public void Cancel(string reason, string? authorizedBy = null)
    {
        if (Status == SaleStatus.Cancelled)
            throw new InvalidSaleException("Sale is already cancelled");

        if (string.IsNullOrWhiteSpace(reason))
            throw new InvalidSaleException("Cancellation reason is required");

        var wasCompleted = Status == SaleStatus.Completed;

        Status = SaleStatus.Cancelled;
        CancelledAt = DateTimeOffset.UtcNow;
        CancellationReason = reason;
        IncrementVersion();

        AddDomainEvent(new SaleCancelledEvent(
            Id,
            TransactionNumber,
            StoreId.Value,
            reason,
            authorizedBy,
            wasCompleted,
            _items.Select(i => new SaleCancelledEvent.ItemInfo(i.Sku.Value, i.Quantity, i.WarehouseId, i.LocationId)).ToList()));
    }

    private void RecalculateTotals()
    {
        var currency = SubTotal.Currency;

        SubTotal = _items.Aggregate(
            Money.Zero(currency),
            (total, item) => total.Add(item.UnitPrice.Multiply(item.Quantity)));

        TaxAmount = _items.Aggregate(
            Money.Zero(currency),
            (total, item) => total.Add(item.TaxAmount));

        var itemDiscounts = _items.Aggregate(
            Money.Zero(currency),
            (total, item) => total.Add(item.DiscountAmount));

        TotalAmount = SubTotal.Add(TaxAmount).Subtract(DiscountAmount).Subtract(itemDiscounts);

        if (TotalAmount.IsNegative)
            TotalAmount = Money.Zero(currency);
    }

    private void EnsureCanModify()
    {
        if (Status == SaleStatus.Completed)
            throw new InvalidSaleException("Cannot modify completed sale");

        if (Status == SaleStatus.Cancelled)
            throw new InvalidSaleException("Cannot modify cancelled sale");
    }

    private static string GenerateTransactionNumber(StoreId storeId, TerminalId terminalId)
    {
        var date = DateTime.UtcNow;
        var random = Random.Shared.Next(1000, 9999);
        return $"{storeId.Value}-{terminalId.Value}-{date:yyyyMMddHHmmss}-{random}";
    }
}

/// <summary>
/// Sale status enumeration
/// </summary>
public enum SaleStatus
{
    Draft = 0,
    PendingCompletion = 1,
    Completed = 2,
    Cancelled = 3
}
