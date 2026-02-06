namespace POS.Application.DTOs;

/// <summary>
/// Payment data transfer object
/// </summary>
public sealed record PaymentDto
{
    public required Guid Id { get; init; }
    public required string PaymentMethod { get; init; }
    public required decimal Amount { get; init; }
    public string? Reference { get; init; }
    public required string Status { get; init; }
    public required DateTimeOffset ProcessedAt { get; init; }
}
