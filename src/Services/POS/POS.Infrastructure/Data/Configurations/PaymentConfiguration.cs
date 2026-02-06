using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using POS.Domain.Entities;

namespace POS.Infrastructure.Data.Configurations;

public sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id");

        builder.Property(p => p.SaleId)
            .HasColumnName("sale_id")
            .IsRequired();

        builder.Property(p => p.PaymentMethod)
            .HasColumnName("payment_method")
            .HasMaxLength(50)
            .IsRequired();

        builder.OwnsOne(p => p.Amount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("amount")
                .HasPrecision(18, 2)
                .IsRequired();
            money.Property(m => m.Currency)
                .HasColumnName("currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(p => p.Reference)
            .HasColumnName("reference")
            .HasMaxLength(100);

        builder.Property(p => p.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(p => p.ProcessedAt)
            .HasColumnName("processed_at")
            .IsRequired();
    }
}
