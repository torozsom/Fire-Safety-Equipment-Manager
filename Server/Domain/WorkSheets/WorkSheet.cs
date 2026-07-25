using Domain.Common;
using Domain.WorkSheets.Enums;
using Domain.WorkSheets.Events;
using Domain.WorkSheets.Exceptions;
using Domain.WorkSheets.ValueObjects;

namespace Domain.WorkSheets;

/// <summary>
///     Represents the work sheet domain model.
/// </summary>
public sealed class WorkSheet : AuditableEntity
{
    /// <summary>
    ///     Initializes a new instance of the WorkSheet class for persistence.
    /// </summary>
    private WorkSheet()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the WorkSheet class.
    /// </summary>
    public WorkSheet(
        Guid id,
        string workSheetNumber,
        Guid serviceReportId,
        CompanySnapshot issuerCompany,
        CompanySnapshot customerCompany,
        SiteSnapshot site,
        int versionNumber,
        DateTimeOffset createdAt,
        Guid? createdByUserId = null)
        : base(id, createdAt, createdByUserId)
    {
        if (serviceReportId == Guid.Empty) throw new WorkSheetCannotBeIssuedException("Service report id is required.");

        WorkSheetNumber = Required(workSheetNumber, nameof(workSheetNumber));
        ServiceReportId = serviceReportId;
        IssuerCompany = issuerCompany;
        CustomerCompany = customerCompany;
        Site = site;
        VersionNumber = versionNumber > 0
            ? versionNumber
            : throw new WorkSheetCannotBeIssuedException("Version number must be greater than zero.");
        Status = WorkSheetStatus.Draft;
    }

    /// <summary>
    ///     Gets the work sheet number value.
    /// </summary>
    public string WorkSheetNumber { get; private set; } = string.Empty;

    /// <summary>
    ///     Gets the service report id value.
    /// </summary>
    public Guid ServiceReportId { get; }

    /// <summary>
    ///     Gets the status value.
    /// </summary>
    public WorkSheetStatus Status { get; private set; }

    /// <summary>
    ///     Gets the issued at value.
    /// </summary>
    public DateTimeOffset? IssuedAt { get; private set; }

    /// <summary>
    ///     Gets the issued by user id value.
    /// </summary>
    public Guid? IssuedByUserId { get; private set; }

    /// <summary>
    ///     Gets the issuer company value.
    /// </summary>
    public CompanySnapshot IssuerCompany { get; private set; } = null!;

    /// <summary>
    ///     Gets the customer company value.
    /// </summary>
    public CompanySnapshot CustomerCompany { get; private set; } = null!;

    /// <summary>
    ///     Gets the site value.
    /// </summary>
    public SiteSnapshot Site { get; private set; } = null!;

    /// <summary>
    ///     Gets the work started at value.
    /// </summary>
    public DateTimeOffset? WorkStartedAt { get; private set; }

    /// <summary>
    ///     Gets the work completed at value.
    /// </summary>
    public DateTimeOffset? WorkCompletedAt { get; private set; }

    /// <summary>
    ///     Gets the work summary value.
    /// </summary>
    public string? WorkSummary { get; private set; }

    /// <summary>
    ///     Gets the general findings value.
    /// </summary>
    public string? GeneralFindings { get; private set; }

    /// <summary>
    ///     Gets the customer representative name value.
    /// </summary>
    public string? CustomerRepresentativeName { get; private set; }

    /// <summary>
    ///     Gets the customer representative title value.
    /// </summary>
    public string? CustomerRepresentativeTitle { get; private set; }

    /// <summary>
    ///     Gets the accepted at value.
    /// </summary>
    public DateTimeOffset? AcceptedAt { get; private set; }

    /// <summary>
    ///     Gets the version number value.
    /// </summary>
    public int VersionNumber { get; private set; }

    /// <summary>
    ///     Gets the replaces work sheet id value.
    /// </summary>
    public Guid? ReplacesWorkSheetId { get; private set; }

    /// <summary>
    ///     Gets the generated document id value.
    /// </summary>
    public Guid? GeneratedDocumentId { get; private set; }

    /// <summary>
    ///     Executes the capture work details domain operation.
    /// </summary>
    public void CaptureWorkDetails(
        DateTimeOffset? workStartedAt,
        DateTimeOffset workCompletedAt,
        string workSummary,
        string? generalFindings,
        string customerRepresentativeName,
        string? customerRepresentativeTitle,
        Guid? updatedByUserId,
        DateTimeOffset updatedAt)
    {
        EnsureDraft();
        WorkStartedAt = workStartedAt;
        WorkCompletedAt = workCompletedAt;
        WorkSummary = Required(workSummary, nameof(workSummary));
        GeneralFindings = Optional(generalFindings);
        CustomerRepresentativeName = Required(customerRepresentativeName, nameof(customerRepresentativeName));
        CustomerRepresentativeTitle = Optional(customerRepresentativeTitle);
        MarkUpdated(updatedByUserId, updatedAt);
    }

    /// <summary>
    ///     Executes the issue domain operation.
    /// </summary>
    public void Issue(Guid issuedByUserId, DateTimeOffset issuedAt, Guid? generatedDocumentId)
    {
        EnsureDraft();
        if (WorkCompletedAt is null || string.IsNullOrWhiteSpace(WorkSummary) ||
            string.IsNullOrWhiteSpace(CustomerRepresentativeName))
            throw new WorkSheetCannotBeIssuedException("Worksheet work details must be completed before issuing.");

        Status = WorkSheetStatus.Issued;
        IssuedAt = issuedAt;
        IssuedByUserId = issuedByUserId;
        GeneratedDocumentId = generatedDocumentId;
        MarkUpdated(issuedByUserId, issuedAt);
        AddDomainEvent(new WorkSheetIssuedDomainEvent(Id, ServiceReportId, issuedAt));
    }

    /// <summary>
    ///     Executes the accept domain operation.
    /// </summary>
    public void Accept(Guid acceptedByUserId, DateTimeOffset acceptedAt)
    {
        if (Status != WorkSheetStatus.Issued)
            throw new WorkSheetCannotBeIssuedException("Only issued worksheets can be accepted.");

        Status = WorkSheetStatus.Accepted;
        AcceptedAt = acceptedAt;
        MarkUpdated(acceptedByUserId, acceptedAt);
    }

    /// <summary>
    ///     Executes the supersede domain operation.
    /// </summary>
    public void Supersede(Guid replacingWorkSheetId, Guid supersededByUserId, DateTimeOffset supersededAt)
    {
        if (replacingWorkSheetId == Guid.Empty)
            throw new WorkSheetCannotBeIssuedException("Replacing worksheet id is required.");

        Status = WorkSheetStatus.Superseded;
        MarkUpdated(supersededByUserId, supersededAt);
        AddDomainEvent(new WorkSheetSupersededDomainEvent(Id, replacingWorkSheetId, supersededAt));
    }

    /// <summary>
    ///     Executes the set replacement domain operation.
    /// </summary>
    public void SetReplacement(Guid replacedWorkSheetId)
    {
        ReplacesWorkSheetId = replacedWorkSheetId;
    }

    /// <summary>
    ///     Executes the ensure draft domain operation.
    /// </summary>
    private void EnsureDraft()
    {
        if (Status != WorkSheetStatus.Draft)
            throw new WorkSheetCannotBeIssuedException("Only draft worksheets can be modified.");
    }

    /// <summary>
    ///     Executes the required domain operation.
    /// </summary>
    private static string Required(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new WorkSheetCannotBeIssuedException($"{parameterName} is required.");

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