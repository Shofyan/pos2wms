using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WMS.Domain.Entities;

namespace WMS.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for InventoryTransaction entity
/// </summary>
public sealed class InventoryTransactionConfiguration : IEntityTypeConfiguration<InventoryTransaction>
{
    public void Configure(EntityTypeBuilder<InventoryTransaction> builder)
    {
        builder.ToTable("inventory_transactions");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasColumnName("id");

        builder.Property(t => t.InventoryId)
            .HasColumnName("inventory_id")
            .IsRequired();
        builder.HasIndex(t => t.InventoryId);

        builder.Property(t => t.Sku)
            .HasColumnName("sku")
            .HasMaxLength(50)
            .IsRequired();
        builder.HasIndex(t => t.Sku);

        builder.Property(t => t.WarehouseId)
            .HasColumnName("warehouse_id")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.Type)
            .HasColumnName("type")
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(t => t.Quantity)
            .HasColumnName("quantity")
            .IsRequired();

        builder.Property(t => t.QuantityBefore)
            .HasColumnName("quantity_before")
            .IsRequired();

        builder.Property(t => t.QuantityAfter)
            .HasColumnName("quantity_after")
            .IsRequired();

        builder.Property(t => t.Reason)
            .HasColumnName("reason")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(t => t.ReferenceId)
            .HasColumnName("reference_id")
            .HasMaxLength(100)
            .IsRequired();
        builder.HasIndex(t => t.ReferenceId);

        builder.Property(t => t.ReferenceType)
            .HasColumnName("reference_type")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.SourceEventId)
            .HasColumnName("source_event_id")
            .HasMaxLength(100);

        builder.Property(t => t.TransactionDate)
            .HasColumnName("transaction_date")
            .IsRequired();
        builder.HasIndex(t => t.TransactionDate);

        builder.Property(t => t.CreatedBy)
            .HasColumnName("created_by")
            .HasMaxLength(100);
    }
}
