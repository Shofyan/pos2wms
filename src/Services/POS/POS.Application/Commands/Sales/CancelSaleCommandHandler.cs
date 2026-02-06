using Common.PostgreSQL.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Logging;
using POS.Domain.Entities;
using POS.Domain.Exceptions;
using POS.Domain.Repositories;

namespace POS.Application.Commands.Sales;

/// <summary>
/// Handler for CancelSaleCommand
/// </summary>
public sealed class CancelSaleCommandHandler : IRequestHandler<CancelSaleCommand, CancelSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CancelSaleCommandHandler> _logger;

    public CancelSaleCommandHandler(
        ISaleRepository saleRepository,
        IUnitOfWork unitOfWork,
        ILogger<CancelSaleCommandHandler> logger)
    {
        _saleRepository = saleRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<CancelSaleResult> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cancelling sale {SaleId}. Reason: {Reason}", request.SaleId, request.Reason);

        var sale = await _saleRepository.GetByIdWithDetailsAsync(request.SaleId, cancellationToken)
            ?? throw new InvalidSaleException($"Sale {request.SaleId} not found");

        var wasCompleted = sale.Status == SaleStatus.Completed;

        sale.Cancel(request.Reason, request.AuthorizedBy);

        _saleRepository.Update(sale);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Sale {SaleId} cancelled. Was completed: {WasCompleted}",
            sale.Id, wasCompleted);

        return new CancelSaleResult
        {
            SaleId = sale.Id,
            TransactionNumber = sale.TransactionNumber,
            CancelledAt = sale.CancelledAt!.Value,
            WasCompleted = wasCompleted
        };
    }
}
