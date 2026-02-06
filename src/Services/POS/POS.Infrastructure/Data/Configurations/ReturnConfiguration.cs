using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.Entities;
using POS.Domain.ValueObjects;

namespace POS.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for Return entity
/// </summary>
public sealed class ReturnConfiguration : IEntityTypeConfiguration<Return>
{
    public void Configure(EntityTypeBuilder<Return> builder)
    {
        builder.ToTable("returns");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).HasColumnName("id");

        builder.Property(r => r.ReturnNumber)
            .HasColumnName("return_number")
            .HasMaxLength(100)
            .IsRequired();
        builder.HasIndex(r => r.ReturnNumber).IsUnique();

        builder.Property(r => r.OriginalSaleId)
            .HasColumnName("original_sale_id")
            .IsRequired();
        builder.HasIndex(r => r.OriginalSaleId);

        builder.Property(r => r.OriginalTransactionNumber)
            .HasColumnName("original_transaction_number")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(r => r.StoreId)
            .HasColumnName("store_id")
            .HasMaxLength(20)
            .HasConversion(v => v.Value, v => StoreId.Create(v))
            .IsRequired();

        builder.Property(r => r.TerminalId)
            .HasColumnName("terminal_id")
            .HasMaxLength(20)
            .HasConversion(v => v.Value, v => TerminalId.Create(v))
            .IsRequired();

        builder.Property(r => r.CashierId)
            .HasColumnName("cashier_id")
            .HasMaxLength(50);

        builder.Property(r => r.CustomerId)
            .HasColumnName("customer_id")
            .HasMaxLength(50);

        builder.Property(r => r.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(r => r.ReturnReason)
            .HasColumnName("return_reason")
            .HasMaxLength(500)
            .IsRequired();

        builder.OwnsOne(r => r.RefundAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("refund_amount")
                .HasPrecision(18, 2)
                .IsRequired();
            money.Property(m => m.Currency)
                .HasColumnName("currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(r => r.RefundMethod)
            .HasColumnName("refund_method")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(r => r.ProcessedAt)
            .HasColumnName("processed_at");

        builder.Property(r => r.Version)
            .HasColumnName("version")
            .IsConcurrencyToken();

        builder.Property(r => r.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(r => r.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.HasMany(r => r.Items)
            .WithOne(i => i.Return)
            .HasForeignKey(i => i.ReturnId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(r => r.DomainEvents);
    }
}
