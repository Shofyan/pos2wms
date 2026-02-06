using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.Entities;
using POS.Domain.ValueObjects;

namespace POS.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for Sale entity
/// </summary>
public sealed class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("sales");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnName("id");

        builder.Property(s => s.TransactionNumber)
            .HasColumnName("transaction_number")
            .HasMaxLength(100)
            .IsRequired();
        builder.HasIndex(s => s.TransactionNumber).IsUnique();

        builder.Property(s => s.StoreId)
            .HasColumnName("store_id")
            .HasMaxLength(20)
            .HasConversion(v => v.Value, v => StoreId.Create(v))
            .IsRequired();
        builder.HasIndex(s => s.StoreId);

        builder.Property(s => s.TerminalId)
            .HasColumnName("terminal_id")
            .HasMaxLength(20)
            .HasConversion(v => v.Value, v => TerminalId.Create(v))
            .IsRequired();

        builder.Property(s => s.CashierId)
            .HasColumnName("cashier_id")
            .HasMaxLength(50);

        builder.Property(s => s.CustomerId)
            .HasColumnName("customer_id")
            .HasMaxLength(50);

        builder.Property(s => s.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
        builder.HasIndex(s => s.Status);

        // Money value objects
        builder.OwnsOne(s => s.SubTotal, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("sub_total")
                .HasPrecision(18, 2)
                .IsRequired();
            money.Property(m => m.Currency)
                .HasColumnName("currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.OwnsOne(s => s.TaxAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("tax_amount")
                .HasPrecision(18, 2)
                .IsRequired();
            money.Ignore(m => m.Currency);
        });

        builder.OwnsOne(s => s.DiscountAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("discount_amount")
                .HasPrecision(18, 2)
                .IsRequired();
            money.Ignore(m => m.Currency);
        });

        builder.OwnsOne(s => s.TotalAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("total_amount")
                .HasPrecision(18, 2)
                .IsRequired();
            money.Ignore(m => m.Currency);
        });

        builder.OwnsOne(s => s.PaidAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("paid_amount")
                .HasPrecision(18, 2)
                .IsRequired();
            money.Ignore(m => m.Currency);
        });

        builder.OwnsOne(s => s.ChangeAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("change_amount")
                .HasPrecision(18, 2)
                .IsRequired();
            money.Ignore(m => m.Currency);
        });

        builder.Property(s => s.CompletedAt)
            .HasColumnName("completed_at");
        builder.HasIndex(s => s.CompletedAt);

        builder.Property(s => s.CancelledAt)
            .HasColumnName("cancelled_at");

        builder.Property(s => s.CancellationReason)
            .HasColumnName("cancellation_reason")
            .HasMaxLength(500);

        builder.Property(s => s.Version)
            .HasColumnName("version")
            .IsConcurrencyToken();

        builder.Property(s => s.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.HasIndex(s => s.CreatedAt);

        builder.Property(s => s.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        // Relationships
        builder.HasMany(s => s.Items)
            .WithOne(i => i.Sale)
            .HasForeignKey(i => i.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Payments)
            .WithOne(p => p.Sale)
            .HasForeignKey(p => p.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore domain events
        builder.Ignore(s => s.DomainEvents);
    }
}
