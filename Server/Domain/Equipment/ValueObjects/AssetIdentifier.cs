using Domain.Common;

namespace Domain.Equipment.ValueObjects;

/// <summary>
///     Represents the asset identifier domain model.
/// </summary>
public sealed class AssetIdentifier : ValueObject
{
    /// <summary>
    ///     Initializes a new instance of the AssetIdentifier class for persistence.
    /// </summary>
    private AssetIdentifier()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the AssetIdentifier class.
    /// </summary>
    public AssetIdentifier(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException("Asset identifier is required.");

        Value = value.Trim();
    }

    /// <summary>
    ///     Gets the value value.
    /// </summary>
    public string Value { get; } = string.Empty;

    /// <summary>
    ///     Returns the string representation of this value.
    /// </summary>
    public override string ToString()
    {
        return Value;
    }

    /// <summary>
    ///     Executes the get equality components domain operation.
    /// </summary>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value.ToUpperInvariant();
    }
}