using Domain.Common;

namespace Domain.CustomerCompanies.Events;

/// <summary>
///     Represents the customer company archived domain event domain event data.
/// </summary>
public sealed record CustomerCompanyArchivedDomainEvent(Guid CustomerCompanyId, DateTimeOffset ArchivedAt)
    : IDomainEvent
{
    /// <summary>
    ///     Gets the id value.
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    ///     Gets the occurred at value.
    /// </summary>
    public DateTimeOffset OccurredAt { get; } = ArchivedAt;
}