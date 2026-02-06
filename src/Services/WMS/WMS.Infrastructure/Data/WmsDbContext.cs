using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;

namespace WMS.Infrastructure.Data;

/// <summary>
/// WMS Database Context
/// </summary>
public sealed class WmsDbContext : DbContext
{
    public WmsDbContext(DbContextOptions<WmsDbContext> options) : base(options)
    {
    }

    public DbSet<Inventory> Inventories => Set<Inventory>();
    public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("wms");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WmsDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
