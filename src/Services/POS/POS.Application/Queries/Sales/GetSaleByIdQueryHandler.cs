using MediatR;
using POS.Application.DTOs;
using POS.Domain.Repositories;

namespace POS.Application.Queries.Sales;

/// <summary>
/// Handler for GetSaleByIdQuery
/// </summary>
public sealed class GetSaleByIdQueryHandler : IRequestHandler<GetSaleByIdQuery, SaleDto?>
{
    private readonly ISaleRepository _saleRepository;

    public GetSaleByIdQueryHandler(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    public async Task<SaleDto?> Handle(GetSaleByIdQuery request, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdWithDetailsAsync(request.SaleId, cancellationToken);
        if (sale is null) return null;

        return new SaleDto
        {
            Id = sale.Id,
            TransactionNumber = sale.TransactionNumber,
            StoreId = sale.StoreId.Value,
            TerminalId = sale.TerminalId.Value,
            CashierId = sale.CashierId,
            CustomerId = sale.CustomerId,
            Status = sale.Status.ToString(),
            SubTotal = sale.SubTotal.Amount,
            TaxAmount = sale.TaxAmount.Amount,
            DiscountAmount = sale.DiscountAmount.Amount,
            TotalAmount = sale.TotalAmount.Amount,
            PaidAmount = sale.PaidAmount.Amount,
            ChangeAmount = sale.ChangeAmount.Amount,
            Currency = sale.TotalAmount.Currency,
            CompletedAt = sale.CompletedAt,
            CancelledAt = sale.CancelledAt,
            CancellationReason = sale.CancellationReason,
            CreatedAt = sale.CreatedAt,
            UpdatedAt = sale.UpdatedAt,
            Items = sale.Items.Select(i => new SaleItemDto
            {
                Id = i.Id,
                Sku = i.Sku.Value,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice.Amount,
                TaxRate = i.TaxRate,
                TaxAmount = i.TaxAmount.Amount,
                DiscountAmount = i.DiscountAmount.Amount,
                TotalPrice = i.TotalPrice.Amount,
                WarehouseId = i.WarehouseId,
                LocationId = i.LocationId
            }).ToList(),
            Payments = sale.Payments.Select(p => new PaymentDto
            {
                Id = p.Id,
                PaymentMethod = p.PaymentMethod,
                Amount = p.Amount.Amount,
                Reference = p.Reference,
                Status = p.Status.ToString(),
                ProcessedAt = p.ProcessedAt
            }).ToList()
        };
    }
}
