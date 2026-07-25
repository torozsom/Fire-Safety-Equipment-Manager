namespace Domain.Common;

/// <summary>
///     Represents the auditable entity domain model.
/// </summary>
public abstract class AuditableEntity : AggregateRoot
{
    /// <summary>
    ///     Initializes a new instance of the AuditableEntity class.
    /// </summary>
    protected AuditableEntity()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the AuditableEntity class.
    /// </summary>
    protected AuditableEntity(Guid id, DateTimeOffset createdAt, Guid? createdByUserId)
        : base(id)
    {
        CreatedAt = createdAt;
        CreatedByUserId = createdByUserId;
    }

    /// <summary>
    ///     Gets the created at value.
    /// </summary>
    public DateTimeOffset CreatedAt { get; protected set; }

    /// <summary>
    ///     Gets the created by user id value.
    /// </summary>
    public Guid? CreatedByUserId { get; protected set; }

    /// <summary>
    ///     Gets the updated at value.
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; protected set; }

    /// <summary>
    ///     Gets the updated by user id value.
    /// </summary>
    public Guid? UpdatedByUserId { get; protected set; }

    /// <summary>
    ///     Gets the row version value.
    /// </summary>
    public byte[] RowVersion { get; protected set; } = [];

    /// <summary>
    ///     Executes the mark updated domain operation.
    /// </summary>
    protected void MarkUpdated(Guid? updatedByUserId, DateTimeOffset updatedAt)
    {
        UpdatedByUserId = updatedByUserId;
        UpdatedAt = updatedAt;
    }
}