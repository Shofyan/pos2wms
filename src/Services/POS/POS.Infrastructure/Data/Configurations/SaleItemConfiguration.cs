using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.Entities;
using POS.Domain.ValueObjects;

namespace POS.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for SaleItem entity
/// </summary>
public sealed class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("sale_items");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).HasColumnName("id");

        builder.Property(i => i.SaleId)
            .HasColumnName("sale_id")
            .IsRequired();
        builder.HasIndex(i => i.SaleId);

        builder.Property(i => i.Sku)
            .HasColumnName("sku")
            .HasMaxLength(50)
            .HasConversion(v => v.Value, v => SKU.Create(v))
            .IsRequired();
        builder.HasIndex(i => i.Sku);

        builder.Property(i => i.ProductName)
            .HasColumnName("product_name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(i => i.Quantity)
            .HasColumnName("quantity")
            .IsRequired();

        builder.OwnsOne(i => i.UnitPrice, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("unit_price")
                .HasPrecision(18, 2)
                .IsRequired();
            money.Property(m => m.Currency)
                .HasColumnName("currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(i => i.TaxRate)
            .HasColumnName("tax_rate")
            .HasPrecision(5, 4)
            .IsRequired();

        builder.OwnsOne(i => i.TaxAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("tax_amount")
                .HasPrecision(18, 2)
                .IsRequired();
            money.Ignore(m => m.Currency);
        });

        builder.OwnsOne(i => i.DiscountAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("discount_amount")
                .HasPrecision(18, 2)
                .IsRequired();
            money.Ignore(m => m.Currency);
        });

        builder.OwnsOne(i => i.TotalPrice, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("total_price")
                .HasPrecision(18, 2)
                .IsRequired();
            money.Ignore(m => m.Currency);
        });

        builder.Property(i => i.WarehouseId)
            .HasColumnName("warehouse_id")
            .HasMaxLength(50);

        builder.Property(i => i.LocationId)
            .HasColumnName("location_id")
            .HasMaxLength(50);
    }
}
