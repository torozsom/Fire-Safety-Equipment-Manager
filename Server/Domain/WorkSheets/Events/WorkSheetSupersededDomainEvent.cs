using Domain.Common;

namespace Domain.WorkSheets.Events;

/// <summary>
///     Represents the work sheet superseded domain event domain event data.
/// </summary>
public sealed record WorkSheetSupersededDomainEvent(
    Guid WorkSheetId,
    Guid ReplacedByWorkSheetId,
    DateTimeOffset SupersededAt) : IDomainEvent
{
    /// <summary>
    ///     Gets the id value.
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    ///     Gets the occurred at value.
    /// </summary>
    public DateTimeOffset OccurredAt { get; } = SupersededAt;
}