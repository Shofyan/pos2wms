using MediatR;
using Microsoft.AspNetCore.Mvc;
using POS.Application.Commands.Sales;
using POS.Application.Queries.Sales;

namespace POS.API.Controllers;

/// <summary>
/// Sales management endpoints
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public sealed class SalesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<SalesController> _logger;

    public SalesController(IMediator mediator, ILogger<SalesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Create a new sale
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateSaleResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateSaleResult>> CreateSale(
        [FromBody] CreateSaleCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating sale for store {StoreId}", command.StoreId);

        var result = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(
            nameof(GetSale),
            new { id = result.SaleId },
            result);
    }

    /// <summary>
    /// Get a sale by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Application.DTOs.SaleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Application.DTOs.SaleDto>> GetSale(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetSaleByIdQuery(id), cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Get sales by store
    /// </summary>
    [HttpGet("store/{storeId}")]
    [ProducesResponseType(typeof(PagedResult<Application.DTOs.SaleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<Application.DTOs.SaleDto>>> GetSalesByStore(
        string storeId,
        [FromQuery] DateTimeOffset? from,
        [FromQuery] DateTimeOffset? to,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var query = new GetSalesByStoreQuery
        {
            StoreId = storeId,
            From = from,
            To = to,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Complete a sale with payment
    /// </summary>
    [HttpPost("{id:guid}/complete")]
    [ProducesResponseType(typeof(CompleteSaleResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CompleteSaleResult>> CompleteSale(
        Guid id,
        [FromBody] CompletePaymentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CompleteSaleCommand
        {
            SaleId = id,
            Payments = request.Payments
        };

        var result = await _mediator.Send(command, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Cancel a sale
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(typeof(CancelSaleResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CancelSaleResult>> CancelSale(
        Guid id,
        [FromBody] CancelSaleRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CancelSaleCommand
        {
            SaleId = id,
            Reason = request.Reason,
            AuthorizedBy = request.AuthorizedBy
        };

        var result = await _mediator.Send(command, cancellationToken);

        return Ok(result);
    }
}

public sealed record CompletePaymentRequest
{
    public required IReadOnlyList<PaymentRequest> Payments { get; init; }
}

public sealed record CancelSaleRequest
{
    public required string Reason { get; init; }
    public string? AuthorizedBy { get; init; }
}
