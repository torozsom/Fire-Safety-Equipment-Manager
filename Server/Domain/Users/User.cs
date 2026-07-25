using Domain.Common;
using Domain.Users.Enums;
using Domain.Users.Events;

namespace Domain.Users;

/// <summary>
///     Represents the user domain model.
/// </summary>
public sealed class User : AuditableEntity
{
    /// <summary>
    ///     Initializes a new instance of the User class for persistence.
    /// </summary>
    private User()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the User class.
    /// </summary>
    public User(
        Guid id,
        string email,
        string firstName,
        string lastName,
        SystemRole systemRole,
        DateTimeOffset createdAt,
        Guid? createdByUserId = null)
        : base(id, createdAt, createdByUserId)
    {
        Email = Required(email, nameof(email));
        NormalizedEmail = NormalizeEmail(email);
        FirstName = Required(firstName, nameof(firstName));
        LastName = Required(lastName, nameof(lastName));
        SystemRole = systemRole;
        Status = UserStatus.Invited;
    }

    /// <summary>
    ///     Gets the email value.
    /// </summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>
    ///     Gets the normalized email value.
    /// </summary>
    public string NormalizedEmail { get; private set; } = string.Empty;

    /// <summary>
    ///     Gets the first name value.
    /// </summary>
    public string FirstName { get; private set; } = string.Empty;

    /// <summary>
    ///     Gets the last name value.
    /// </summary>
    public string LastName { get; private set; } = string.Empty;

    /// <summary>
    ///     Gets the phone number value.
    /// </summary>
    public string? PhoneNumber { get; private set; }

    /// <summary>
    ///     Gets the system role value.
    /// </summary>
    public SystemRole SystemRole { get; private set; }

    /// <summary>
    ///     Gets the status value.
    /// </summary>
    public UserStatus Status { get; private set; }

    /// <summary>
    ///     Gets the email confirmed value.
    /// </summary>
    public bool EmailConfirmed { get; private set; }

    /// <summary>
    ///     Gets the last login at value.
    /// </summary>
    public DateTimeOffset? LastLoginAt { get; private set; }

    /// <summary>
    ///     Gets the deactivated at value.
    /// </summary>
    public DateTimeOffset? DeactivatedAt { get; private set; }

    /// <summary>
    ///     Gets the deactivated by user id value.
    /// </summary>
    public Guid? DeactivatedByUserId { get; private set; }

    /// <summary>
    ///     Gets the deactivation reason value.
    /// </summary>
    public string? DeactivationReason { get; private set; }

    /// <summary>
    ///     Gets the full name value.
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>
    ///     Executes the activate domain operation.
    /// </summary>
    public void Activate(DateTimeOffset activatedAt, Guid? activatedByUserId)
    {
        if (Status == UserStatus.Active) return;

        Status = UserStatus.Active;
        DeactivatedAt = null;
        DeactivatedByUserId = null;
        DeactivationReason = null;
        MarkUpdated(activatedByUserId, activatedAt);
        AddDomainEvent(new UserActivatedDomainEvent(Id, activatedAt));
    }

    /// <summary>
    ///     Executes the suspend domain operation.
    /// </summary>
    public void Suspend(Guid? suspendedByUserId, DateTimeOffset suspendedAt)
    {
        Status = UserStatus.Suspended;
        MarkUpdated(suspendedByUserId, suspendedAt);
    }

    /// <summary>
    ///     Executes the deactivate domain operation.
    /// </summary>
    public void Deactivate(string reason, Guid? deactivatedByUserId, DateTimeOffset deactivatedAt)
    {
        Status = UserStatus.Deactivated;
        DeactivatedAt = deactivatedAt;
        DeactivatedByUserId = deactivatedByUserId;
        DeactivationReason = Required(reason, nameof(reason));
        MarkUpdated(deactivatedByUserId, deactivatedAt);
    }

    /// <summary>
    ///     Executes the confirm email domain operation.
    /// </summary>
    public void ConfirmEmail(Guid? confirmedByUserId, DateTimeOffset confirmedAt)
    {
        EmailConfirmed = true;
        MarkUpdated(confirmedByUserId, confirmedAt);
    }

    /// <summary>
    ///     Executes the record login domain operation.
    /// </summary>
    public void RecordLogin(DateTimeOffset loginAt)
    {
        LastLoginAt = loginAt;
    }

    /// <summary>
    ///     Executes the update profile domain operation.
    /// </summary>
    public void UpdateProfile(string firstName, string lastName, string? phoneNumber, Guid? updatedByUserId,
        DateTimeOffset updatedAt)
    {
        FirstName = Required(firstName, nameof(firstName));
        LastName = Required(lastName, nameof(lastName));
        PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber.Trim();
        MarkUpdated(updatedByUserId, updatedAt);
    }

    /// <summary>
    ///     Executes the change email domain operation.
    /// </summary>
    public void ChangeEmail(string email, Guid? updatedByUserId, DateTimeOffset updatedAt)
    {
        Email = Required(email, nameof(email));
        NormalizedEmail = NormalizeEmail(email);
        EmailConfirmed = false;
        MarkUpdated(updatedByUserId, updatedAt);
    }

    /// <summary>
    ///     Executes the normalize email domain operation.
    /// </summary>
    private static string NormalizeEmail(string email)
    {
        return Required(email, nameof(email)).ToUpperInvariant();
    }

    /// <summary>
    ///     Executes the required domain operation.
    /// </summary>
    private static string Required(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException($"{parameterName} is required.");

        return value.Trim();
    }
}