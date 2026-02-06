using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WMS.Domain.Entities;

namespace WMS.Infrastructure.Data;

public static class WmsDbInitializer
{
    public static async Task SeedAsync(WmsDbContext context, ILogger logger)
    {
        try
        {
            if (!await context.Inventories.AnyAsync())
            {
                logger.LogInformation("Seeding WMS initial inventory data...");

                var inventories = new List<Inventory>
                {
                    Inventory.Create("SKU-12345", "WH-001", "LOC-01", 100, 10, 50),
                    Inventory.Create("SKU-67890", "WH-001", "LOC-02", 50, 5, 20),
                    Inventory.Create("SKU-APPLE", "WH-001", "LOC-03", 200, 20, 100),
                    Inventory.Create("SKU-ORANGE", "WH-001", "LOC-04", 150, 15, 75)
                };

                await context.Inventories.AddRangeAsync(inventories);
                await context.SaveChangesAsync();

                logger.LogInformation("WMS inventory data seeded successfully.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the WMS database.");
        }
    }
}
