using MediatR;
using POS.Application.DTOs;

namespace POS.Application.Queries.Sales;

/// <summary>
/// Query to get sales by store with pagination
/// </summary>
public sealed record GetSalesByStoreQuery : IRequest<PagedResult<SaleDto>>
{
    public required string StoreId { get; init; }
    public DateTimeOffset? From { get; init; }
    public DateTimeOffset? To { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}

/// <summary>
/// Paged result wrapper
/// </summary>
public sealed record PagedResult<T>
{
    public required IReadOnlyList<T> Items { get; init; }
    public required int TotalCount { get; init; }
    public required int PageNumber { get; init; }
    public required int PageSize { get; init; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;
}
