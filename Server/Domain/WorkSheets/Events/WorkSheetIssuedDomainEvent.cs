using Domain.Common;

namespace Domain.WorkSheets.Events;

/// <summary>
///     Represents the work sheet issued domain event domain event data.
/// </summary>
public sealed record WorkSheetIssuedDomainEvent(Guid WorkSheetId, Guid ServiceReportId, DateTimeOffset IssuedAt)
    : IDomainEvent
{
    /// <summary>
    ///     Gets the id value.
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    ///     Gets the occurred at value.
    /// </summary>
    public DateTimeOffset OccurredAt { get; } = IssuedAt;
}