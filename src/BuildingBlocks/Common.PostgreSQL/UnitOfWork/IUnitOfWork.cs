namespace Common.PostgreSQL.UnitOfWork;

/// <summary>
/// Unit of Work pattern interface
/// </summary>
public interface IUnitOfWork : IAsyncDisposable
{
    /// <summary>
    /// Begin a transaction
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commit the current transaction
    /// </summary>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rollback the current transaction
    /// </summary>
    Task RollbackAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Save all changes to the database
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Whether there is an active transaction
    /// </summary>
    bool HasActiveTransaction { get; }
}
