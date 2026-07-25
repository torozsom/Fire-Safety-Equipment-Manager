using Domain.Common;

namespace Domain.Users.Events;

/// <summary>
///     Represents the user activated domain event domain event data.
/// </summary>
public sealed record UserActivatedDomainEvent(Guid UserId, DateTimeOffset ActivatedAt) : IDomainEvent
{
    /// <summary>
    ///     Gets the id value.
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    ///     Gets the occurred at value.
    /// </summary>
    public DateTimeOffset OccurredAt { get; } = ActivatedAt;
}