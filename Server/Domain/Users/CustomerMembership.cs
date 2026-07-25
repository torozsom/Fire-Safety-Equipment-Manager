using Domain.Common;
using Domain.Users.Enums;
using Domain.Users.Events;
using Domain.Users.Exceptions;

namespace Domain.Users;

/// <summary>
///     Represents the customer membership domain model.
/// </summary>
public sealed class CustomerMembership : AuditableEntity
{
    /// <summary>
    ///     Initializes a new instance of the CustomerMembership class for persistence.
    /// </summary>
    private CustomerMembership()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the CustomerMembership class.
    /// </summary>
    public CustomerMembership(
        Guid id,
        Guid userId,
        Guid customerCompanyId,
        CustomerMembershipRole role,
        DateTimeOffset createdAt,
        Guid? createdByUserId = null)
        : base(id, createdAt, createdByUserId)
    {
        if (userId == Guid.Empty) throw new InvalidCustomerMembershipException("User id is required.");

        if (customerCompanyId == Guid.Empty)
            throw new InvalidCustomerMembershipException("Customer company id is required.");

        UserId = userId;
        CustomerCompanyId = customerCompanyId;
        Role = role;
        Status = CustomerMembershipStatus.Invited;

        AddDomainEvent(new CustomerMembershipCreatedDomainEvent(Id, UserId, CustomerCompanyId, Role, createdAt));
    }

    /// <summary>
    ///     Gets the user id value.
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    ///     Gets the customer company id value.
    /// </summary>
    public Guid CustomerCompanyId { get; }

    /// <summary>
    ///     Gets the role value.
    /// </summary>
    public CustomerMembershipRole Role { get; private set; }

    /// <summary>
    ///     Gets the status value.
    /// </summary>
    public CustomerMembershipStatus Status { get; private set; }

    /// <summary>
    ///     Gets the activated at value.
    /// </summary>
    public DateTimeOffset? ActivatedAt { get; private set; }

    /// <summary>
    ///     Gets the activated by user id value.
    /// </summary>
    public Guid? ActivatedByUserId { get; private set; }

    /// <summary>
    ///     Gets the suspended at value.
    /// </summary>
    public DateTimeOffset? SuspendedAt { get; private set; }

    /// <summary>
    ///     Gets the suspended by user id value.
    /// </summary>
    public Guid? SuspendedByUserId { get; private set; }

    /// <summary>
    ///     Gets the deactivated at value.
    /// </summary>
    public DateTimeOffset? DeactivatedAt { get; private set; }

    /// <summary>
    ///     Gets the deactivated by user id value.
    /// </summary>
    public Guid? DeactivatedByUserId { get; private set; }

    /// <summary>
    ///     Gets the note value.
    /// </summary>
    public string? Note { get; private set; }

    /// <summary>
    ///     Executes the activate domain operation.
    /// </summary>
    public void Activate(Guid? activatedByUserId, DateTimeOffset activatedAt)
    {
        Status = CustomerMembershipStatus.Active;
        ActivatedAt = activatedAt;
        ActivatedByUserId = activatedByUserId;
        SuspendedAt = null;
        SuspendedByUserId = null;
        MarkUpdated(activatedByUserId, activatedAt);
    }

    /// <summary>
    ///     Executes the change role domain operation.
    /// </summary>
    public void ChangeRole(CustomerMembershipRole role, Guid? updatedByUserId, DateTimeOffset updatedAt)
    {
        Role = role;
        MarkUpdated(updatedByUserId, updatedAt);
    }

    /// <summary>
    ///     Executes the suspend domain operation.
    /// </summary>
    public void Suspend(Guid? suspendedByUserId, DateTimeOffset suspendedAt, string? note = null)
    {
        Status = CustomerMembershipStatus.Suspended;
        SuspendedAt = suspendedAt;
        SuspendedByUserId = suspendedByUserId;
        Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        MarkUpdated(suspendedByUserId, suspendedAt);
    }

    /// <summary>
    ///     Executes the deactivate domain operation.
    /// </summary>
    public void Deactivate(Guid? deactivatedByUserId, DateTimeOffset deactivatedAt, string? note = null)
    {
        Status = CustomerMembershipStatus.Deactivated;
        DeactivatedAt = deactivatedAt;
        DeactivatedByUserId = deactivatedByUserId;
        Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        MarkUpdated(deactivatedByUserId, deactivatedAt);
    }
}