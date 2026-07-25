using Domain.Common;

namespace Domain.Servicing.Events;

/// <summary>
///     Represents the service report completed domain event domain event data.
/// </summary>
public sealed record ServiceReportCompletedDomainEvent(Guid ServiceReportId, Guid SiteId, DateTimeOffset CompletedAt)
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