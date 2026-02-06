using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Common.PostgreSQL.Interceptors;

/// <summary>
/// Interceptor for audit fields (created/updated timestamps)
/// </summary>
public sealed class AuditInterceptor : SaveChangesInterceptor
{
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<AuditInterceptor> _logger;

    public AuditInterceptor(TimeProvider timeProvider, ILogger<AuditInterceptor> logger)
    {
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        UpdateAuditFields(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateAuditFields(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateAuditFields(DbContext? context)
    {
        if (context is null) return;

        var now = _timeProvider.GetUtcNow();

        foreach (var entry in context.ChangeTracker.Entries<IAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedAt = now;
                    _logger.LogTrace("Set audit fields for new entity: {EntityType}", entry.Entity.GetType().Name);
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    entry.Property(nameof(IAuditableEntity.CreatedAt)).IsModified = false;
                    _logger.LogTrace("Updated audit fields for modified entity: {EntityType}", entry.Entity.GetType().Name);
                    break;
            }
        }
    }
}

/// <summary>
/// Interface for entities with audit fields
/// </summary>
public interface IAuditableEntity
{
    DateTimeOffset CreatedAt { get; set; }
    DateTimeOffset UpdatedAt { get; set; }
}
