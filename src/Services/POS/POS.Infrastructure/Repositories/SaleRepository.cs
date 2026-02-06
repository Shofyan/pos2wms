using Microsoft.EntityFrameworkCore;
using POS.Domain.Entities;
using POS.Domain.Repositories;
using POS.Infrastructure.Data;

namespace POS.Infrastructure.Repositories;

/// <summary>
/// Sale repository implementation
/// </summary>
public sealed class SaleRepository : ISaleRepository
{
    private readonly PosDbContext _context;

    public SaleRepository(PosDbContext context)
    {
        _context = context;
    }

    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Sale?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Sale?> GetByTransactionNumberAsync(string transactionNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Include(s => s.Payments)
            .FirstOrDefaultAsync(s => s.TransactionNumber == transactionNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<Sale>> GetByStoreIdAsync(
        string storeId,
        DateTimeOffset from,
        DateTimeOffset to,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Where(s => s.StoreId.Value == storeId)
            .Where(s => s.CreatedAt >= from && s.CreatedAt <= to)
            .OrderByDescending(s => s.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Sale>> GetPendingSalesAsync(
        string storeId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Where(s => s.StoreId.Value == storeId)
            .Where(s => s.Status == SaleStatus.Draft || s.Status == SaleStatus.PendingCompletion)
            .OrderBy(s => s.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _context.Sales.AddAsync(sale, cancellationToken);
    }

    public void Update(Sale sale)
    {
        _context.Sales.Update(sale);
    }

    public void Delete(Sale sale)
    {
        _context.Sales.Remove(sale);
    }

    public async Task<bool> ExistsAsync(string transactionNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .AnyAsync(s => s.TransactionNumber == transactionNumber, cancellationToken);
    }

    public async Task<int> CountByStatusAsync(
        string storeId,
        SaleStatus status,
        DateTimeOffset from,
        DateTimeOffset to,
        CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Where(s => s.StoreId.Value == storeId)
            .Where(s => s.Status == status)
            .Where(s => s.CreatedAt >= from && s.CreatedAt <= to)
            .CountAsync(cancellationToken);
    }
}
