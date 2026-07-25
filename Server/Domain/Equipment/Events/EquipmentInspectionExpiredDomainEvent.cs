using Domain.Common;

namespace Domain.Equipment.Events;

/// <summary>
///     Represents the equipment inspection expired domain event domain event data.
/// </summary>
public sealed record EquipmentInspectionExpiredDomainEvent(
    Guid EquipmentId,
    DateOnly DueDate,
    DateTimeOffset DetectedAt) : IDomainEvent
{
    /// <summary>
    ///     Gets the id value.
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    ///     Gets the occurred at value.
    /// </summary>
    public DateTimeOffset OccurredAt { get; } = DetectedAt;
}