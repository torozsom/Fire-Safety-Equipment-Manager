namespace Domain.Common;

/// <summary>
///     Represents the entity domain model.
/// </summary>
public abstract class Entity
{
    /// <summary>
    ///     Initializes a new instance of the Entity class.
    /// </summary>
    protected Entity()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the Entity class.
    /// </summary>
    protected Entity(Guid id)
    {
        if (id == Guid.Empty) throw new DomainException("Entity id cannot be empty.");

        Id = id;
    }

    /// <summary>
    ///     Gets the id value.
    /// </summary>
    public Guid Id { get; protected set; }

    /// <summary>
    ///     Determines whether this instance is equal to another object.
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is not Entity other) return false;

        if (ReferenceEquals(this, other)) return true;

        return GetType() == other.GetType() && Id != Guid.Empty && Id == other.Id;
    }

    /// <summary>
    ///     Returns a hash code for this instance.
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(GetType(), Id);
    }

    /// <summary>
    ///     Compares entity instances for equality.
    /// </summary>
    public static bool operator ==(Entity? left, Entity? right)
    {
        return left?.Equals(right) ?? right is null;
    }

    /// <summary>
    ///     Compares entity instances for equality.
    /// </summary>
    public static bool operator !=(Entity? left, Entity? right)
    {
        return !(left == right);
    }
}