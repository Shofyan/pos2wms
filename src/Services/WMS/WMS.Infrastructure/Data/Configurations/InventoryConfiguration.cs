using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WMS.Domain.Entities;

namespace WMS.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for Inventory entity
/// </summary>
public sealed class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.ToTable("inventories");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).HasColumnName("id");

        builder.Property(i => i.Sku)
            .HasColumnName("sku")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(i => i.WarehouseId)
            .HasColumnName("warehouse_id")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(i => i.LocationId)
            .HasColumnName("location_id")
            .HasMaxLength(50);

        builder.HasIndex(i => new { i.Sku, i.WarehouseId }).IsUnique();
        builder.HasIndex(i => i.WarehouseId);
        builder.HasIndex(i => i.Sku);

        builder.Property(i => i.QuantityOnHand)
            .HasColumnName("quantity_on_hand")
            .IsRequired();

        builder.Property(i => i.QuantityReserved)
            .HasColumnName("quantity_reserved")
            .IsRequired();

        builder.Property(i => i.ReorderPoint)
            .HasColumnName("reorder_point")
            .IsRequired();

        builder.Property(i => i.ReorderQuantity)
            .HasColumnName("reorder_quantity")
            .IsRequired();

        builder.Property(i => i.LastStockCheck)
            .HasColumnName("last_stock_check")
            .IsRequired();

        builder.Property(i => i.Version)
            .HasColumnName("version")
            .IsConcurrencyToken();

        builder.Property(i => i.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(i => i.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.Ignore(i => i.QuantityAvailable);
        builder.Ignore(i => i.IsLowStock);
        builder.Ignore(i => i.IsOutOfStock);
    }
}
