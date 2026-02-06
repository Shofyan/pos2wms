using Common.PostgreSQL.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Logging;
using POS.Domain.Entities;
using POS.Domain.Exceptions;
using POS.Domain.Repositories;
using POS.Domain.ValueObjects;

namespace POS.Application.Commands.Returns;

/// <summary>
/// Handler for CreateReturnCommand
/// </summary>
public sealed class CreateReturnCommandHandler : IRequestHandler<CreateReturnCommand, CreateReturnResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IReturnRepository _returnRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateReturnCommandHandler> _logger;

    public CreateReturnCommandHandler(
        ISaleRepository saleRepository,
        IReturnRepository returnRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateReturnCommandHandler> logger)
    {
        _saleRepository = saleRepository;
        _returnRepository = returnRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<CreateReturnResult> Handle(CreateReturnCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating return for sale {SaleId}. Reason: {Reason}",
            request.OriginalSaleId, request.ReturnReason);

        var sale = await _saleRepository.GetByIdWithDetailsAsync(request.OriginalSaleId, cancellationToken)
            ?? throw new InvalidReturnException($"Original sale {request.OriginalSaleId} not found");

        var returnEntity = Return.Create(
            sale,
            StoreId.Create(request.StoreId),
            TerminalId.Create(request.TerminalId),
            request.CashierId,
            request.ReturnReason,
            request.RefundMethod);

        foreach (var itemRequest in request.Items)
        {
            var originalItem = sale.Items.FirstOrDefault(i => i.Id == itemRequest.OriginalSaleItemId)
                ?? throw new InvalidReturnException($"Original sale item {itemRequest.OriginalSaleItemId} not found");

            returnEntity.AddItem(
                originalItem,
                itemRequest.Quantity,
                itemRequest.Condition,
                itemRequest.RestockRequired);
        }

        returnEntity.Process();

        await _returnRepository.AddAsync(returnEntity, cancellationToken);

        _logger.LogInformation(
            "Return created with ID {ReturnId}, number {ReturnNumber}. Refund: {RefundAmount}",
            returnEntity.Id, returnEntity.ReturnNumber, returnEntity.RefundAmount.Amount);

        return new CreateReturnResult
        {
            ReturnId = returnEntity.Id,
            ReturnNumber = returnEntity.ReturnNumber,
            RefundAmount = returnEntity.RefundAmount.Amount
        };
    }
}
