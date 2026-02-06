using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Common.PostgreSQL.Extensions;

/// <summary>
/// Database migration extensions
/// </summary>
public static class MigrationExtensions
{
    /// <summary>
    /// Apply pending migrations on application startup
    /// </summary>
    public static async Task MigrateDatabaseAsync<TContext>(
        this IHost host,
        CancellationToken cancellationToken = default) where TContext : DbContext
    {
        using var scope = host.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<TContext>>();
        var context = scope.ServiceProvider.GetRequiredService<TContext>();

        try
        {
            logger.LogInformation("Starting database migration for {Context}", typeof(TContext).Name);

            var pendingMigrations = await context.Database.GetPendingMigrationsAsync(cancellationToken);
            var migrationsList = pendingMigrations.ToList();

            if (migrationsList.Count > 0)
            {
                logger.LogInformation(
                    "Applying {Count} pending migrations: {Migrations}",
                    migrationsList.Count,
                    string.Join(", ", migrationsList));

                await context.Database.MigrateAsync(cancellationToken);

                logger.LogInformation("Database migration completed successfully");
            }
            else
            {
                logger.LogInformation("No pending migrations found");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database");
            throw;
        }
    }

    /// <summary>
    /// Ensure database is created (for development only)
    /// </summary>
    public static async Task EnsureDatabaseCreatedAsync<TContext>(
        this IHost host,
        CancellationToken cancellationToken = default) where TContext : DbContext
    {
        using var scope = host.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<TContext>>();
        var context = scope.ServiceProvider.GetRequiredService<TContext>();

        try
        {
            logger.LogInformation("Ensuring database is created for {Context}", typeof(TContext).Name);

            var created = await context.Database.EnsureCreatedAsync(cancellationToken);

            if (created)
            {
                logger.LogInformation("Database was created");
            }
            else
            {
                logger.LogInformation("Database already exists");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while ensuring database is created");
            throw;
        }
    }
}
