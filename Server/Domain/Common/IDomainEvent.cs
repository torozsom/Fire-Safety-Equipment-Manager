namespace Domain.Common;

/// <summary>
///     Defines the contract for i domain event.
/// </summary>
public interface IDomainEvent
{
    Guid Id { get; }
    DateTimeOffset OccurredAt { get; }
}