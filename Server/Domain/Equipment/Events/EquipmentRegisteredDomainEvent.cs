using Domain.Common;

namespace Domain.Equipment.Events;

/// <summary>
///     Represents the equipment registered domain event domain event data.
/// </summary>
public sealed record EquipmentRegisteredDomainEvent(Guid EquipmentId, Guid SiteId, DateTimeOffset RegisteredAt)
    : IDomainEvent
{
    /// <summary>
    ///     Gets the id value.
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    ///     Gets the occurred at value.
    /// </summary>
    public DateTimeOffset OccurredAt { get; } = RegisteredAt;
}