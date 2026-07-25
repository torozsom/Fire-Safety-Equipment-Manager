using Domain.Common;
using Domain.CustomerCompanies.Enums;
using Domain.CustomerCompanies.ValueObjects;

namespace Domain.CustomerCompanies;

/// <summary>
///     Represents the site domain model.
/// </summary>
public sealed class Site : AuditableEntity
{
    /// <summary>
    ///     Initializes a new instance of the Site class for persistence.
    /// </summary>
    private Site()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the Site class.
    /// </summary>
    public Site(Guid id, Guid customerCompanyId, string name, Address address, DateTimeOffset createdAt,
        Guid? createdByUserId = null)
        : base(id, createdAt, createdByUserId)
    {
        if (customerCompanyId == Guid.Empty) throw new DomainException("Customer company id is required.");

        CustomerCompanyId = customerCompanyId;
        Name = Required(name, nameof(name));
        Address = address;
        Status = SiteStatus.Active;
    }

    /// <summary>
    ///     Gets the customer company id value.
    /// </summary>
    public Guid CustomerCompanyId { get; private set; }

    /// <summary>
    ///     Gets the name value.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    ///     Gets the code value.
    /// </summary>
    public string? Code { get; private set; }

    /// <summary>
    ///     Gets the status value.
    /// </summary>
    public SiteStatus Status { get; private set; }

    /// <summary>
    ///     Gets the address value.
    /// </summary>
    public Address Address { get; private set; } = null!;

    /// <summary>
    ///     Gets the contact name value.
    /// </summary>
    public string? ContactName { get; private set; }

    /// <summary>
    ///     Gets the contact email value.
    /// </summary>
    public string? ContactEmail { get; private set; }

    /// <summary>
    ///     Gets the contact phone number value.
    /// </summary>
    public string? ContactPhoneNumber { get; private set; }

    /// <summary>
    ///     Gets the notes value.
    /// </summary>
    public string? Notes { get; private set; }

    /// <summary>
    ///     Gets the archived at value.
    /// </summary>
    public DateTimeOffset? ArchivedAt { get; private set; }

    /// <summary>
    ///     Gets the archived by user id value.
    /// </summary>
    public Guid? ArchivedByUserId { get; private set; }

    /// <summary>
    ///     Executes the update details domain operation.
    /// </summary>
    public void UpdateDetails(string name, string? code, Address address, string? notes, Guid? updatedByUserId,
        DateTimeOffset updatedAt)
    {
        EnsureNotArchived();
        Name = Required(name, nameof(name));
        Code = Optional(code);
        Address = address;
        Notes = Optional(notes);
        MarkUpdated(updatedByUserId, updatedAt);
    }

    /// <summary>
    ///     Executes the update contact domain operation.
    /// </summary>
    public void UpdateContact(string? name, string? email, string? phoneNumber, Guid? updatedByUserId,
        DateTimeOffset updatedAt)
    {
        EnsureNotArchived();
        ContactName = Optional(name);
        ContactEmail = Optional(email);
        ContactPhoneNumber = Optional(phoneNumber);
        MarkUpdated(updatedByUserId, updatedAt);
    }

    /// <summary>
    ///     Executes the deactivate domain operation.
    /// </summary>
    public void Deactivate(Guid? updatedByUserId, DateTimeOffset updatedAt)
    {
        EnsureNotArchived();
        Status = SiteStatus.Inactive;
        MarkUpdated(updatedByUserId, updatedAt);
    }

    /// <summary>
    ///     Executes the reactivate domain operation.
    /// </summary>
    public void Reactivate(Guid? updatedByUserId, DateTimeOffset updatedAt)
    {
        EnsureNotArchived();
        Status = SiteStatus.Active;
        MarkUpdated(updatedByUserId, updatedAt);
    }

    /// <summary>
    ///     Executes the archive domain operation.
    /// </summary>
    public void Archive(Guid? archivedByUserId, DateTimeOffset archivedAt)
    {
        Status = SiteStatus.Archived;
        ArchivedAt = archivedAt;
        ArchivedByUserId = archivedByUserId;
        MarkUpdated(archivedByUserId, archivedAt);
    }

    /// <summary>
    ///     Executes the ensure not archived domain operation.
    /// </summary>
    private void EnsureNotArchived()
    {
        if (Status == SiteStatus.Archived) throw new DomainException("Archived sites cannot be modified.");
    }

    /// <summary>
    ///     Executes the required domain operation.
    /// </summary>
    private static string Required(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException($"{parameterName} is required.");

        return value.Trim();
    }

    /// <summary>
    ///     Executes the optional domain operation.
    /// </summary>
    private static string? Optional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}