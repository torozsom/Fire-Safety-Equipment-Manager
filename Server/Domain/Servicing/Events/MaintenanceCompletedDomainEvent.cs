using Domain.Common;

namespace Domain.Servicing.Events;

/// <summary>
///     Represents the maintenance completed domain event domain event data.
/// </summary>
public sealed record MaintenanceCompletedDomainEvent(Guid MaintenanceId, Guid EquipmentId, DateTimeOffset CompletedAt)
    : IDomainEvent
{
    /// <summary>
    ///     Gets the id value.
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    ///     Gets the occurred at value.
    /// </summary>
    public DateTimeOffset OccurredAt { get; } = CompletedAt;
}