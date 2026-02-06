using POS.Domain.Common;
using POS.Domain.ValueObjects;

namespace POS.Domain.Entities;

/// <summary>
/// Payment entity representing a payment for a sale
/// </summary>
public sealed class Payment : Entity
{
    public Guid SaleId { get; private set; }
    public Sale Sale { get; private set; } = null!;
    public string PaymentMethod { get; private set; } = string.Empty;
    public Money Amount { get; private set; } = null!;
    public string? Reference { get; private set; }
    public PaymentStatus Status { get; private set; }
    public DateTimeOffset ProcessedAt { get; private set; }

    private Payment() { } // EF Core

    internal static Payment Create(Sale sale, string paymentMethod, Money amount, string? reference)
    {
        return new Payment
        {
            Id = Guid.NewGuid(),
            SaleId = sale.Id,
            Sale = sale,
            PaymentMethod = paymentMethod,
            Amount = amount,
            Reference = reference,
            Status = PaymentStatus.Completed,
            ProcessedAt = DateTimeOffset.UtcNow
        };
    }

    public void MarkAsRefunded()
    {
        Status = PaymentStatus.Refunded;
    }
}

/// <summary>
/// Payment status enumeration
/// </summary>
public enum PaymentStatus
{
    Pending = 0,
    Completed = 1,
    Failed = 2,
    Refunded = 3
}
