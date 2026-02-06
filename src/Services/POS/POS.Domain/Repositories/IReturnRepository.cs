using POS.Domain.Entities;

namespace POS.Domain.Repositories;

/// <summary>
/// Repository interface for Return aggregate
/// </summary>
public interface IReturnRepository
{
    /// <summary>
    /// Get a return by ID
    /// </summary>
    Task<Return?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a return by ID with items
    /// </summary>
    Task<Return?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a return by return number
    /// </summary>
    Task<Return?> GetByReturnNumberAsync(string returnNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get returns by original sale ID
    /// </summary>
    Task<IReadOnlyList<Return>> GetByOriginalSaleIdAsync(
        Guid saleId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get returns by store ID
    /// </summary>
    Task<IReadOnlyList<Return>> GetByStoreIdAsync(
        string storeId,
        DateTimeOffset from,
        DateTimeOffset to,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Add a new return
    /// </summary>
    Task AddAsync(Return returnEntity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing return
    /// </summary>
    void Update(Return returnEntity);

    /// <summary>
    /// Get total refund amount for a sale
    /// </summary>
    Task<decimal> GetTotalRefundAmountBySaleIdAsync(
        Guid saleId,
        CancellationToken cancellationToken = default);
}
