using Domain.Common;
using Domain.CustomerCompanies.Enums;
using Domain.CustomerCompanies.Events;
using Domain.CustomerCompanies.ValueObjects;

namespace Domain.CustomerCompanies;

/// <summary>
///     Represents the customer company domain model.
/// </summary>
public sealed class CustomerCompany : AuditableEntity
{
    /// <summary>
    ///     Stores the sites backing collection.
    /// </summary>
    private readonly List<Site> _sites = [];

    /// <summary>
    ///     Initializes a new instance of the CustomerCompany class for persistence.
    /// </summary>
    private CustomerCompany()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the CustomerCompany class.
    /// </summary>
    public CustomerCompany(Guid id, string name, Address? billingAddress, DateTimeOffset createdAt,
        Guid? createdByUserId = null)
        : base(id, createdAt, createdByUserId)
    {
        Name = Required(name, nameof(name));
        BillingAddress = billingAddress;
        Status = CustomerCompanyStatus.Active;
    }

    /// <summary>
    ///     Gets the name value.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    ///     Gets the legal name value.
    /// </summary>
    public string? LegalName { get; private set; }

    /// <summary>
    ///     Gets the company registration number value.
    /// </summary>
    public string? CompanyRegistrationNumber { get; private set; }

    /// <summary>
    ///     Gets the tax number value.
    /// </summary>
    public string? TaxNumber { get; private set; }

    /// <summary>
    ///     Gets the status value.
    /// </summary>
    public CustomerCompanyStatus Status { get; private set; }

    /// <summary>
    ///     Gets the billing email value.
    /// </summary>
    public string? BillingEmail { get; private set; }

    /// <summary>
    ///     Gets the phone number value.
    /// </summary>
    public string? PhoneNumber { get; private set; }

    /// <summary>
    ///     Gets the website url value.
    /// </summary>
    public string? WebsiteUrl { get; private set; }

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
    ///     Gets the billing address value.
    /// </summary>
    public Address? BillingAddress { get; private set; }

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
    ///     Gets the archive reason value.
    /// </summary>
    public string? ArchiveReason { get; private set; }

    /// <summary>
    ///     Gets the sites value.
    /// </summary>
    public IReadOnlyCollection<Site> Sites => _sites.AsReadOnly();

    /// <summary>
    ///     Executes the update company details domain operation.
    /// </summary>
    public void UpdateCompanyDetails(
        string name,
        string? legalName,
        string? companyRegistrationNumber,
        string? taxNumber,
        Address? billingAddress,
        Guid? updatedByUserId,
        DateTimeOffset updatedAt)
    {
        EnsureNotArchived();
        Name = Required(name, nameof(name));
        LegalName = Optional(legalName);
        CompanyRegistrationNumber = Optional(companyRegistrationNumber);
        TaxNumber = Optional(taxNumber);
        BillingAddress = billingAddress;
        MarkUpdated(updatedByUserId, updatedAt);
    }

    /// <summary>
    ///     Executes the update contact details domain operation.
    /// </summary>
    public void UpdateContactDetails(
        string? billingEmail,
        string? phoneNumber,
        string? websiteUrl,
        string? contactName,
        string? contactEmail,
        string? contactPhoneNumber,
        Guid? updatedByUserId,
        DateTimeOffset updatedAt)
    {
        EnsureNotArchived();
        BillingEmail = Optional(billingEmail);
        PhoneNumber = Optional(phoneNumber);
        WebsiteUrl = Optional(websiteUrl);
        ContactName = Optional(contactName);
        ContactEmail = Optional(contactEmail);
        ContactPhoneNumber = Optional(contactPhoneNumber);
        MarkUpdated(updatedByUserId, updatedAt);
    }

    /// <summary>
    ///     Executes the suspend domain operation.
    /// </summary>
    public void Suspend(Guid? suspendedByUserId, DateTimeOffset suspendedAt)
    {
        EnsureNotArchived();
        Status = CustomerCompanyStatus.Suspended;
        MarkUpdated(suspendedByUserId, suspendedAt);
    }

    /// <summary>
    ///     Executes the reactivate domain operation.
    /// </summary>
    public void Reactivate(Guid? reactivatedByUserId, DateTimeOffset reactivatedAt)
    {
        EnsureNotArchived();
        Status = CustomerCompanyStatus.Active;
        MarkUpdated(reactivatedByUserId, reactivatedAt);
    }

    /// <summary>
    ///     Executes the archive domain operation.
    /// </summary>
    public void Archive(string reason, Guid? archivedByUserId, DateTimeOffset archivedAt)
    {
        if (Status == CustomerCompanyStatus.Archived) return;

        Status = CustomerCompanyStatus.Archived;
        ArchivedAt = archivedAt;
        ArchivedByUserId = archivedByUserId;
        ArchiveReason = Required(reason, nameof(reason));
        MarkUpdated(archivedByUserId, archivedAt);
        AddDomainEvent(new CustomerCompanyArchivedDomainEvent(Id, archivedAt));
    }

    /// <summary>
    ///     Executes the add site domain operation.
    /// </summary>
    public Site AddSite(Guid siteId, string name, Address address, DateTimeOffset createdAt,
        Guid? createdByUserId = null)
    {
        EnsureNotArchived();
        var site = new Site(siteId, Id, name, address, createdAt, createdByUserId);
        _sites.Add(site);
        return site;
    }

    /// <summary>
    ///     Executes the ensure not archived domain operation.
    /// </summary>
    private void EnsureNotArchived()
    {
        if (Status == CustomerCompanyStatus.Archived)
            throw new DomainException("Archived customer companies cannot be modified.");
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