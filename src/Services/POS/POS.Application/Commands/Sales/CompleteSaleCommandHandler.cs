using Common.PostgreSQL.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Logging;
using POS.Domain.Exceptions;
using POS.Domain.Repositories;
using POS.Domain.ValueObjects;

namespace POS.Application.Commands.Sales;

/// <summary>
/// Handler for CompleteSaleCommand
/// </summary>
public sealed class CompleteSaleCommandHandler : IRequestHandler<CompleteSaleCommand, CompleteSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CompleteSaleCommandHandler> _logger;

    public CompleteSaleCommandHandler(
        ISaleRepository saleRepository,
        IUnitOfWork unitOfWork,
        ILogger<CompleteSaleCommandHandler> logger)
    {
        _saleRepository = saleRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<CompleteSaleResult> Handle(CompleteSaleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Completing sale {SaleId}", request.SaleId);

        var sale = await _saleRepository.GetByIdWithDetailsAsync(request.SaleId, cancellationToken)
            ?? throw new InvalidSaleException($"Sale {request.SaleId} not found");

        var currency = sale.TotalAmount.Currency;

        foreach (var payment in request.Payments)
        {
            sale.AddPayment(
                payment.PaymentMethod,
                Money.Create(payment.Amount, currency),
                payment.Reference);
        }

        sale.Complete();

        _saleRepository.Update(sale);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Sale {SaleId} completed. Total: {Total}, Paid: {Paid}, Change: {Change}",
            sale.Id, sale.TotalAmount.Amount, sale.PaidAmount.Amount, sale.ChangeAmount.Amount);

        return new CompleteSaleResult
        {
            SaleId = sale.Id,
            TransactionNumber = sale.TransactionNumber,
            TotalAmount = sale.TotalAmount.Amount,
            PaidAmount = sale.PaidAmount.Amount,
            ChangeAmount = sale.ChangeAmount.Amount,
            CompletedAt = sale.CompletedAt!.Value
        };
    }
}
