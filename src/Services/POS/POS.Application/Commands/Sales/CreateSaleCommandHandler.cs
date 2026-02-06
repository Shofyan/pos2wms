using Common.PostgreSQL.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Logging;
using POS.Domain.Entities;
using POS.Domain.Repositories;
using POS.Domain.ValueObjects;

namespace POS.Application.Commands.Sales;

/// <summary>
/// Handler for CreateSaleCommand
/// </summary>
public sealed class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateSaleCommandHandler> _logger;

    public CreateSaleCommandHandler(
        ISaleRepository saleRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateSaleCommandHandler> logger)
    {
        _saleRepository = saleRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<CreateSaleResult> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating sale for store {StoreId}, terminal {TerminalId}",
            request.StoreId, request.TerminalId);

        var sale = Sale.Create(
            StoreId.Create(request.StoreId),
            TerminalId.Create(request.TerminalId),
            request.CashierId,
            request.CustomerId,
            request.Currency);

        foreach (var item in request.Items)
        {
            sale.AddItem(
                item.Sku,
                item.ProductName,
                item.Quantity,
                Money.Create(item.UnitPrice, request.Currency),
                item.TaxRate,
                item.DiscountAmount > 0 ? Money.Create(item.DiscountAmount, request.Currency) : null,
                item.WarehouseId,
                item.LocationId);
        }

        await _saleRepository.AddAsync(sale, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Sale created with ID {SaleId}, transaction {TransactionNumber}",
            sale.Id, sale.TransactionNumber);

        return new CreateSaleResult
        {
            SaleId = sale.Id,
            TransactionNumber = sale.TransactionNumber,
            TotalAmount = sale.TotalAmount.Amount
        };
    }
}
