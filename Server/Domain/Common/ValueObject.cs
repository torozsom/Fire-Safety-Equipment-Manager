namespace Domain.Common;

/// <summary>
///     Represents the value object domain model.
/// </summary>
public abstract class ValueObject
{
    /// <summary>
    ///     Returns the components used to compare value object equality.
    /// </summary>
    protected abstract IEnumerable<object?> GetEqualityComponents();

    /// <summary>
    ///     Determines whether this instance is equal to another object.
    /// </summary>
    public override bool Equals(object? obj)
    {
        return obj is ValueObject other
               && GetType() == other.GetType()
               && GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    /// <summary>
    ///     Returns a hash code for this instance.
    /// </summary>
    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Aggregate(1, (current, component) => HashCode.Combine(current, component));
    }
}