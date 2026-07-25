using Domain.Common;
using Domain.Users.Enums;

namespace Domain.Users.Events;

/// <summary>
///     Represents the customer membership created domain event domain event data.
/// </summary>
public sealed record CustomerMembershipCreatedDomainEvent(
    Guid CustomerMembershipId,
    Guid UserId,
    Guid CustomerCompanyId,
    CustomerMembershipRole Role,
    DateTimeOffset CreatedAt) : IDomainEvent
{
    /// <summary>
    ///     Gets the id value.
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    ///     Gets the occurred at value.
    /// </summary>
    public DateTimeOffset OccurredAt { get; } = CreatedAt;
}