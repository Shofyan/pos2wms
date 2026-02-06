using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.Entities;
using POS.Domain.ValueObjects;

namespace POS.Infrastructure.Data.Configurations;

public sealed class ReturnItemConfiguration : IEntityTypeConfiguration<ReturnItem>
{
    public void Configure(EntityTypeBuilder<ReturnItem> builder)
    {
        builder.ToTable("return_items");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).HasColumnName("id");

        builder.Property(i => i.ReturnId)
            .HasColumnName("return_id")
            .IsRequired();

        builder.Property(i => i.OriginalSaleItemId)
            .HasColumnName("original_sale_item_id")
            .IsRequired();

        builder.Property(i => i.Sku)
            .HasColumnName("sku")
            .HasMaxLength(50)
            .HasConversion(v => v.Value, v => SKU.Create(v))
            .IsRequired();

        builder.Property(i => i.ProductName)
            .HasColumnName("product_name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(i => i.Quantity)
            .HasColumnName("quantity")
            .IsRequired();

        builder.OwnsOne(i => i.RefundAmount, money =>
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

        builder.Property(i => i.Condition)
            .HasColumnName("condition")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(i => i.WarehouseId)
            .HasColumnName("warehouse_id")
            .HasMaxLength(50);

        builder.Property(i => i.LocationId)
            .HasColumnName("location_id")
            .HasMaxLength(50);

        builder.Property(i => i.RestockRequired)
            .HasColumnName("restock_required")
            .IsRequired();
    }
}
