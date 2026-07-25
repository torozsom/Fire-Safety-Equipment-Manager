using Domain.Common;
using Domain.Servicing.Enums;

namespace Domain.Servicing.Events;

/// <summary>
///     Represents the issue reported domain event domain event data.
/// </summary>
public sealed record IssueReportedDomainEvent(
    Guid IssueId,
    Guid EquipmentId,
    IssueSeverity Severity,
    DateTimeOffset ReportedAt) : IDomainEvent
{
    /// <summary>
    ///     Gets the id value.
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    ///     Gets the occurred at value.
    /// </summary>
    public DateTimeOffset OccurredAt { get; } = ReportedAt;
}