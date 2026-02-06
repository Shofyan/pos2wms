using POS.Domain.Entities;

namespace POS.Domain.Repositories;

/// <summary>
/// Repository interface for Sale aggregate
/// </summary>
public interface ISaleRepository
{
    /// <summary>
    /// Get a sale by ID
    /// </summary>
    Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a sale by ID with items and payments
    /// </summary>
    Task<Sale?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a sale by transaction number
    /// </summary>
    Task<Sale?> GetByTransactionNumberAsync(string transactionNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get sales by store ID
    /// </summary>
    Task<IReadOnlyList<Sale>> GetByStoreIdAsync(
        string storeId,
        DateTimeOffset from,
        DateTimeOffset to,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get pending sales (draft or pending completion)
    /// </summary>
    Task<IReadOnlyList<Sale>> GetPendingSalesAsync(
        string storeId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Add a new sale
    /// </summary>
    Task AddAsync(Sale sale, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing sale
    /// </summary>
    void Update(Sale sale);

    /// <summary>
    /// Delete a sale
    /// </summary>
    void Delete(Sale sale);

    /// <summary>
    /// Check if a transaction number exists
    /// </summary>
    Task<bool> ExistsAsync(string transactionNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get sales count by status
    /// </summary>
    Task<int> CountByStatusAsync(
        string storeId,
        SaleStatus status,
        DateTimeOffset from,
        DateTimeOffset to,
        CancellationToken cancellationToken = default);
}
