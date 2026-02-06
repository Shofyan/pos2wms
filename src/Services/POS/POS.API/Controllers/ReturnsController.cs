using MediatR;
using Microsoft.AspNetCore.Mvc;
using POS.Application.Commands.Returns;

namespace POS.API.Controllers;

/// <summary>
/// Returns management endpoints
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public sealed class ReturnsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ReturnsController> _logger;

    public ReturnsController(IMediator mediator, ILogger<ReturnsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Create a new return
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateReturnResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateReturnResult>> CreateReturn(
        [FromBody] CreateReturnCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating return for sale {SaleId}. Reason: {Reason}",
            command.OriginalSaleId, command.ReturnReason);

        var result = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(
            nameof(GetReturn),
            new { id = result.ReturnId },
            result);
    }

    /// <summary>
    /// Get a return by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Application.DTOs.ReturnDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Application.DTOs.ReturnDto>> GetReturn(
        Guid id,
        CancellationToken cancellationToken)
    {
        // TODO: Implement GetReturnByIdQuery
        return NotFound();
    }
}
