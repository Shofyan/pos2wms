using Microsoft.EntityFrameworkCore;

namespace Common.PostgreSQL.Extensions;

/// <summary>
/// DbContext extension methods
/// </summary>
public static class DbContextExtensions
{
    /// <summary>
    /// Execute a raw SQL query with retry logic
    /// </summary>
    public static async Task<int> ExecuteSqlWithRetryAsync(
        this DbContext context,
        FormattableString sql,
        int maxRetries = 3,
        CancellationToken cancellationToken = default)
    {
        var attempts = 0;
        var delay = TimeSpan.FromMilliseconds(100);

        while (true)
        {
            try
            {
                return await context.Database.ExecuteSqlAsync(sql, cancellationToken);
            }
            catch (Exception) when (attempts++ < maxRetries)
            {
                await Task.Delay(delay, cancellationToken);
                delay *= 2; // Exponential backoff
            }
        }
    }

    /// <summary>
    /// Get the table name for an entity type
    /// </summary>
    public static string? GetTableName<TEntity>(this DbContext context) where TEntity : class
    {
        var entityType = context.Model.FindEntityType(typeof(TEntity));
        return entityType?.GetTableName();
    }

    /// <summary>
    /// Get the schema name for an entity type
    /// </summary>
    public static string? GetSchemaName<TEntity>(this DbContext context) where TEntity : class
    {
        var entityType = context.Model.FindEntityType(typeof(TEntity));
        return entityType?.GetSchema();
    }

    /// <summary>
    /// Check if the database connection is healthy
    /// </summary>
    public static async Task<bool> IsHealthyAsync(
        this DbContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await context.Database.CanConnectAsync(cancellationToken);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Clear all change tracker entries
    /// </summary>
    public static void ClearChangeTracker(this DbContext context)
    {
        context.ChangeTracker.Clear();
    }

    /// <summary>
    /// Detach all entities of a specific type
    /// </summary>
    public static void DetachAll<TEntity>(this DbContext context) where TEntity : class
    {
        var entries = context.ChangeTracker.Entries<TEntity>().ToList();
        foreach (var entry in entries)
        {
            entry.State = EntityState.Detached;
        }
    }
}
