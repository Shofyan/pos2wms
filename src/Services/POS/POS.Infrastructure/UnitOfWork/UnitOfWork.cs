using Common.PostgreSQL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using POS.Infrastructure.Data;

namespace POS.Infrastructure.UnitOfWork;

/// <summary>
/// Unit of Work implementation for POS
/// </summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly PosDbContext _context;
    private IDbContextTransaction? _currentTransaction;
    private bool _disposed;

    public UnitOfWork(PosDbContext context)
    {
        _context = context;
    }

    public bool HasActiveTransaction => _currentTransaction is not null;

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is not null)
        {
            return;
        }

        _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is null)
        {
            return;
        }

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            await _currentTransaction.CommitAsync(cancellationToken);
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction is null)
        {
            return;
        }

        try
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public async Task<T> ExecuteTransactionalAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default)
    {
        var strategy = _context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await BeginTransactionAsync(cancellationToken);
            try
            {
                var result = await operation();
                await CommitAsync(cancellationToken);
                return result;
            }
            catch
            {
                // Rely on TransactionBehavior or caller to handle exceptions, but we must ensure rollback happens here if we rely on this method for transaction boundary.
                // However, CommitAsync handles successful completion.
                // If operation throws, we should rollback.
                
                // Wait, if we use this method, we are taking responsibility for the transaction lifecycle.
                // So we should rollback.
                await RollbackAsync(cancellationToken);
                throw;
            }
        });
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<TResult> ExecuteInTransactionAsync<TResult>(
        Func<CancellationToken, Task<TResult>> operation,
        CancellationToken cancellationToken = default)
    {
        var strategy = _context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async ct =>
        {
            await BeginTransactionAsync(ct);
            try
            {
                var result = await operation(ct);
                await CommitAsync(ct);
                return result;
            }
            catch
            {
                await RollbackAsync(ct);
                throw;
            }
        }, cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        if (_currentTransaction is not null)
        {
            await _currentTransaction.DisposeAsync();
        }

        _disposed = true;
    }
}
