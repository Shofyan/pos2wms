using MediatR;
using POS.Application.DTOs;

namespace POS.Application.Queries.Sales;

/// <summary>
/// Query to get a sale by ID
/// </summary>
public sealed record GetSaleByIdQuery(Guid SaleId) : IRequest<SaleDto?>;
