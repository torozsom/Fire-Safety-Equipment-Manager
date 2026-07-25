using Domain.Common;

namespace Domain.Notifications.ValueObjects;

/// <summary>
///     Represents the deduplication key domain model.
/// </summary>
public sealed class DeduplicationKey : ValueObject
{
    /// <summary>
    ///     Initializes a new instance of the DeduplicationKey class for persistence.
    /// </summary>
    private DeduplicationKey()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the DeduplicationKey class.
    /// </summary>
    public DeduplicationKey(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException("Deduplication key is required.");

        Value = value.Trim();
    }

    /// <summary>
    ///     Gets the value value.
    /// </summary>
    public string Value { get; } = string.Empty;

    /// <summary>
    ///     Executes the get equality components domain operation.
    /// </summary>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value.ToUpperInvariant();
    }
}