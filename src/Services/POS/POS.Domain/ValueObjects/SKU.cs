using System.Text.RegularExpressions;

namespace POS.Domain.ValueObjects;

/// <summary>
/// Value object representing a Stock Keeping Unit (SKU)
/// </summary>
public sealed partial record SKU(string Value)
{
    private SKU() : this(string.Empty) { } // EF Core

    public static SKU Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("SKU cannot be empty", nameof(value));

        var normalized = value.Trim().ToUpperInvariant();

        if (normalized.Length < 3 || normalized.Length > 50)
            throw new ArgumentException("SKU must be between 3 and 50 characters", nameof(value));

        if (!SkuPattern().IsMatch(normalized))
            throw new ArgumentException("SKU can only contain letters, numbers, and hyphens", nameof(value));

        return new SKU(normalized);
    }

    public static bool TryCreate(string value, out SKU? sku)
    {
        try
        {
            sku = Create(value);
            return true;
        }
        catch
        {
            sku = null;
            return false;
        }
    }

    public static implicit operator string(SKU sku) => sku.Value;

    public override string ToString() => Value;

    [GeneratedRegex(@"^[A-Z0-9\-]+$")]
    private static partial Regex SkuPattern();
}
