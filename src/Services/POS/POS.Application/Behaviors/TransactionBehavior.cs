using Common.PostgreSQL.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Logging;

namespace POS.Application.Behaviors;

/// <summary>
/// Pipeline behavior for transaction management
/// </summary>
public sealed class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

    public TransactionBehavior(IUnitOfWork unitOfWork, ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        // Only wrap commands (not queries) in transactions
        if (!requestName.EndsWith("Command"))
        {
            return await next();
        }

        if (_unitOfWork.HasActiveTransaction)
        {
            return await next();
        }

        _logger.LogDebug("Starting transaction for {RequestName}", requestName);

        var response = await _unitOfWork.ExecuteInTransactionAsync(async ct =>
        {
            var result = await next();
            return result;
        }, cancellationToken);
        try
        {
            var response = await _unitOfWork.ExecuteTransactionalAsync(async () =>
            {
                _logger.LogDebug("Started transaction for {RequestName}", requestName);
                return await next();
            }, cancellationToken);

        _logger.LogDebug("Committed transaction for {RequestName}", requestName);

        return response;
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in transaction for {RequestName}", requestName);
            throw;
        }
    }
}
