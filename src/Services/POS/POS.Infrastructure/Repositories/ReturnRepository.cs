using Microsoft.EntityFrameworkCore;
using POS.Domain.Entities;
using POS.Domain.Repositories;
using POS.Infrastructure.Data;

namespace POS.Infrastructure.Repositories;

/// <summary>
/// Return repository implementation
/// </summary>
public sealed class ReturnRepository : IReturnRepository
{
    private readonly PosDbContext _context;

    public ReturnRepository(PosDbContext context)
    {
        _context = context;
    }

    public async Task<Return?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Returns
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<Return?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Returns
            .Include(r => r.Items)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<Return?> GetByReturnNumberAsync(string returnNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Returns
            .Include(r => r.Items)
            .FirstOrDefaultAsync(r => r.ReturnNumber == returnNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<Return>> GetByOriginalSaleIdAsync(
        Guid saleId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Returns
            .Include(r => r.Items)
            .Where(r => r.OriginalSaleId == saleId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Return>> GetByStoreIdAsync(
        string storeId,
        DateTimeOffset from,
        DateTimeOffset to,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        return await _context.Returns
            .Where(r => r.StoreId.Value == storeId)
            .Where(r => r.CreatedAt >= from && r.CreatedAt <= to)
            .OrderByDescending(r => r.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Return returnEntity, CancellationToken cancellationToken = default)
    {
        await _context.Returns.AddAsync(returnEntity, cancellationToken);
    }

    public void Update(Return returnEntity)
    {
        _context.Returns.Update(returnEntity);
    }

    public async Task<decimal> GetTotalRefundAmountBySaleIdAsync(
        Guid saleId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Returns
            .Where(r => r.OriginalSaleId == saleId)
            .Where(r => r.Status == ReturnStatus.Processed)
            .SumAsync(r => r.RefundAmount.Amount, cancellationToken);
    }
}
