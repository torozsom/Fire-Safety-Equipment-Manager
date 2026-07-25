namespace Domain.Common;

/// <summary>
///     Represents the aggregate root domain model.
/// </summary>
public abstract class AggregateRoot : Entity
{
    /// <summary>
    ///     Stores the domain events backing collection.
    /// </summary>
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>
    ///     Initializes a new instance of the AggregateRoot class.
    /// </summary>
    protected AggregateRoot()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the AggregateRoot class.
    /// </summary>
    protected AggregateRoot(Guid id)
        : base(id)
    {
    }

    /// <summary>
    ///     Gets the domain events value.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    ///     Executes the clear domain events domain operation.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    /// <summary>
    ///     Executes the add domain event domain operation.
    /// </summary>
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}