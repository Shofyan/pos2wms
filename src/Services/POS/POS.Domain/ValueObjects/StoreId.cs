namespace POS.Domain.ValueObjects;

/// <summary>
/// Value object representing a Store identifier
/// </summary>
public sealed record StoreId
{
    public string Value { get; }

    private StoreId(string value)
    {
        Value = value;
    }

    public static StoreId Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Store ID cannot be empty", nameof(value));

        var normalized = value.Trim().ToUpperInvariant();

        if (normalized.Length < 2 || normalized.Length > 20)
            throw new ArgumentException("Store ID must be between 2 and 20 characters", nameof(value));

        return new StoreId(normalized);
    }

    public static implicit operator string(StoreId storeId) => storeId.Value;

    public override string ToString() => Value;
}

/// <summary>
/// Value object representing a Terminal identifier
/// </summary>
public sealed record TerminalId
{
    public string Value { get; }

    private TerminalId(string value)
    {
        Value = value;
    }

    public static TerminalId Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Terminal ID cannot be empty", nameof(value));

        var normalized = value.Trim().ToUpperInvariant();

        return new TerminalId(normalized);
    }

    public static implicit operator string(TerminalId terminalId) => terminalId.Value;

    public override string ToString() => Value;
}
