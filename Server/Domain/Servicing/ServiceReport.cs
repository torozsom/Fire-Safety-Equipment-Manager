using Domain.Common;
using Domain.Servicing.Enums;
using Domain.Servicing.Events;
using Domain.Servicing.Exceptions;

namespace Domain.Servicing;

/// <summary>
///     Represents the service report domain model.
/// </summary>
public sealed class ServiceReport : AuditableEntity
{
    /// <summary>
    ///     Initializes a new instance of the ServiceReport class for persistence.
    /// </summary>
    private ServiceReport()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the ServiceReport class.
    /// </summary>
    public ServiceReport(Guid id, string reportNumber, Guid siteId, DateTimeOffset createdAt,
        Guid? createdByUserId = null)
        : base(id, createdAt, createdByUserId)
    {
        if (siteId == Guid.Empty) throw new InvalidServiceReportStateException("Site id is required.");

        ReportNumber = Required(reportNumber, nameof(reportNumber));
        SiteId = siteId;
        Status = ServiceReportStatus.Planned;
    }

    /// <summary>
    ///     Gets the report number value.
    /// </summary>
    public string ReportNumber { get; private set; } = string.Empty;

    /// <summary>
    ///     Gets the site id value.
    /// </summary>
    public Guid SiteId { get; }

    /// <summary>
    ///     Gets the status value.
    /// </summary>
    public ServiceReportStatus Status { get; private set; }

    /// <summary>
    ///     Gets the scheduled start at value.
    /// </summary>
    public DateTimeOffset? ScheduledStartAt { get; private set; }

    /// <summary>
    ///     Gets the scheduled end at value.
    /// </summary>
    public DateTimeOffset? ScheduledEndAt { get; private set; }

    /// <summary>
    ///     Gets the started at value.
    /// </summary>
    public DateTimeOffset? StartedAt { get; private set; }

    /// <summary>
    ///     Gets the completed at value.
    /// </summary>
    public DateTimeOffset? CompletedAt { get; private set; }

    /// <summary>
    ///     Gets the primary technician user id value.
    /// </summary>
    public Guid? PrimaryTechnicianUserId { get; private set; }

    /// <summary>
    ///     Gets the description value.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    ///     Gets the customer representative name value.
    /// </summary>
    public string? CustomerRepresentativeName { get; private set; }

    /// <summary>
    ///     Gets the customer representative title value.
    /// </summary>
    public string? CustomerRepresentativeTitle { get; private set; }

    /// <summary>
    ///     Gets the general findings value.
    /// </summary>
    public string? GeneralFindings { get; private set; }

    /// <summary>
    ///     Gets the notes value.
    /// </summary>
    public string? Notes { get; private set; }

    /// <summary>
    ///     Gets the cancellation reason value.
    /// </summary>
    public string? CancellationReason { get; private set; }

    /// <summary>
    ///     Gets the cancelled by user id value.
    /// </summary>
    public Guid? CancelledByUserId { get; private set; }

    /// <summary>
    ///     Gets the cancelled at value.
    /// </summary>
    public DateTimeOffset? CancelledAt { get; private set; }

    /// <summary>
    ///     Executes the schedule domain operation.
    /// </summary>
    public void Schedule(DateTimeOffset? startAt, DateTimeOffset? endAt, Guid? primaryTechnicianUserId,
        string? description, Guid? updatedByUserId, DateTimeOffset updatedAt)
    {
        EnsureNotClosed();
        if (startAt.HasValue && endAt.HasValue && endAt <= startAt)
            throw new InvalidServiceReportStateException("Scheduled end must be after scheduled start.");

        ScheduledStartAt = startAt;
        ScheduledEndAt = endAt;
        PrimaryTechnicianUserId = primaryTechnicianUserId;
        Description = Optional(description);
        MarkUpdated(updatedByUserId, updatedAt);
    }

    /// <summary>
    ///     Executes the start domain operation.
    /// </summary>
    public void Start(DateTimeOffset startedAt, Guid? startedByUserId)
    {
        if (Status != ServiceReportStatus.Planned)
            throw new InvalidServiceReportStateException("Only planned service reports can be started.");

        Status = ServiceReportStatus.InProgress;
        StartedAt = startedAt;
        MarkUpdated(startedByUserId, startedAt);
    }

    /// <summary>
    ///     Executes the complete domain operation.
    /// </summary>
    public void Complete(
        string customerRepresentativeName,
        string? customerRepresentativeTitle,
        string generalFindings,
        string? notes,
        DateTimeOffset completedAt,
        Guid? completedByUserId)
    {
        if (Status != ServiceReportStatus.InProgress)
            throw new InvalidServiceReportStateException("Only in-progress service reports can be completed.");

        CustomerRepresentativeName = Required(customerRepresentativeName, nameof(customerRepresentativeName));
        CustomerRepresentativeTitle = Optional(customerRepresentativeTitle);
        GeneralFindings = Required(generalFindings, nameof(generalFindings));
        Notes = Optional(notes);
        Status = ServiceReportStatus.Completed;
        CompletedAt = completedAt;
        MarkUpdated(completedByUserId, completedAt);
        AddDomainEvent(new ServiceReportCompletedDomainEvent(Id, SiteId, completedAt));
    }

    /// <summary>
    ///     Executes the cancel domain operation.
    /// </summary>
    public void Cancel(string reason, Guid? cancelledByUserId, DateTimeOffset cancelledAt)
    {
        EnsureNotClosed();
        CancellationReason = Required(reason, nameof(reason));
        Status = ServiceReportStatus.Cancelled;
        CancelledByUserId = cancelledByUserId;
        CancelledAt = cancelledAt;
        MarkUpdated(cancelledByUserId, cancelledAt);
    }

    /// <summary>
    ///     Executes the ensure not closed domain operation.
    /// </summary>
    private void EnsureNotClosed()
    {
        if (Status is ServiceReportStatus.Completed or ServiceReportStatus.Cancelled)
            throw new InvalidServiceReportStateException("Closed service reports cannot be modified.");
    }

    /// <summary>
    ///     Executes the required domain operation.
    /// </summary>
    private static string Required(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidServiceReportStateException($"{parameterName} is required.");

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