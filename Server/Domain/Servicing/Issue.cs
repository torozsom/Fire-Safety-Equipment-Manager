using Domain.Common;
using Domain.Servicing.Enums;
using Domain.Servicing.Events;

namespace Domain.Servicing;

/// <summary>
///     Represents the issue domain model.
/// </summary>
public sealed class Issue : AuditableEntity
{
    /// <summary>
    ///     Initializes a new instance of the Issue class for persistence.
    /// </summary>
    private Issue()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the Issue class.
    /// </summary>
    public Issue(
        Guid id,
        Guid equipmentId,
        string issueNumber,
        string title,
        string description,
        IssueSeverity severity,
        DateTimeOffset reportedAt,
        Guid reportedByUserId)
        : base(id, reportedAt, reportedByUserId)
    {
        if (equipmentId == Guid.Empty || reportedByUserId == Guid.Empty)
            throw new DomainException("Equipment id and reporter id are required.");

        EquipmentId = equipmentId;
        IssueNumber = Required(issueNumber, nameof(issueNumber));
        Title = Required(title, nameof(title));
        Description = Required(description, nameof(description));
        Severity = severity;
        Status = IssueStatus.Open;
        ReportedAt = reportedAt;
        ReportedByUserId = reportedByUserId;
        AddDomainEvent(new IssueReportedDomainEvent(Id, EquipmentId, Severity, ReportedAt));
    }

    /// <summary>
    ///     Gets the equipment id value.
    /// </summary>
    public Guid EquipmentId { get; }

    /// <summary>
    ///     Gets the issue number value.
    /// </summary>
    public string IssueNumber { get; private set; } = string.Empty;

    /// <summary>
    ///     Gets the title value.
    /// </summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>
    ///     Gets the description value.
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    ///     Gets the severity value.
    /// </summary>
    public IssueSeverity Severity { get; }

    /// <summary>
    ///     Gets the status value.
    /// </summary>
    public IssueStatus Status { get; private set; }

    /// <summary>
    ///     Gets the reported at value.
    /// </summary>
    public DateTimeOffset ReportedAt { get; }

    /// <summary>
    ///     Gets the reported by user id value.
    /// </summary>
    public Guid ReportedByUserId { get; private set; }

    /// <summary>
    ///     Gets the assigned to user id value.
    /// </summary>
    public Guid? AssignedToUserId { get; private set; }

    /// <summary>
    ///     Gets the acknowledged at value.
    /// </summary>
    public DateTimeOffset? AcknowledgedAt { get; private set; }

    /// <summary>
    ///     Gets the acknowledged by user id value.
    /// </summary>
    public Guid? AcknowledgedByUserId { get; private set; }

    /// <summary>
    ///     Gets the resolution value.
    /// </summary>
    public string? Resolution { get; private set; }

    /// <summary>
    ///     Gets the resolved at value.
    /// </summary>
    public DateTimeOffset? ResolvedAt { get; private set; }

    /// <summary>
    ///     Gets the resolved by user id value.
    /// </summary>
    public Guid? ResolvedByUserId { get; private set; }

    /// <summary>
    ///     Gets the closed at value.
    /// </summary>
    public DateTimeOffset? ClosedAt { get; private set; }

    /// <summary>
    ///     Gets the closed by user id value.
    /// </summary>
    public Guid? ClosedByUserId { get; private set; }

    /// <summary>
    ///     Gets the rejection reason value.
    /// </summary>
    public string? RejectionReason { get; private set; }

    /// <summary>
    ///     Executes the assign domain operation.
    /// </summary>
    public void Assign(Guid assignedToUserId, Guid? updatedByUserId, DateTimeOffset updatedAt)
    {
        EnsureOpen();
        if (assignedToUserId == Guid.Empty) throw new DomainException("Assigned user id is required.");

        AssignedToUserId = assignedToUserId;
        MarkUpdated(updatedByUserId, updatedAt);
    }

    /// <summary>
    ///     Executes the acknowledge domain operation.
    /// </summary>
    public void Acknowledge(Guid acknowledgedByUserId, DateTimeOffset acknowledgedAt)
    {
        EnsureOpen();
        EnsureUserId(acknowledgedByUserId, nameof(acknowledgedByUserId));
        Status = IssueStatus.Acknowledged;
        AcknowledgedAt = acknowledgedAt;
        AcknowledgedByUserId = acknowledgedByUserId;
        MarkUpdated(acknowledgedByUserId, acknowledgedAt);
    }

    /// <summary>
    ///     Executes the start progress domain operation.
    /// </summary>
    public void StartProgress(Guid? updatedByUserId, DateTimeOffset updatedAt)
    {
        EnsureOpen();
        Status = IssueStatus.InProgress;
        MarkUpdated(updatedByUserId, updatedAt);
    }

    /// <summary>
    ///     Executes the resolve domain operation.
    /// </summary>
    public void Resolve(string resolution, Guid resolvedByUserId, DateTimeOffset resolvedAt)
    {
        EnsureOpen();
        EnsureUserId(resolvedByUserId, nameof(resolvedByUserId));
        Resolution = Required(resolution, nameof(resolution));
        Status = IssueStatus.Resolved;
        ResolvedAt = resolvedAt;
        ResolvedByUserId = resolvedByUserId;
        MarkUpdated(resolvedByUserId, resolvedAt);
    }

    /// <summary>
    ///     Executes the close domain operation.
    /// </summary>
    public void Close(Guid closedByUserId, DateTimeOffset closedAt)
    {
        if (Status != IssueStatus.Resolved) throw new DomainException("Only resolved issues can be closed.");
        EnsureUserId(closedByUserId, nameof(closedByUserId));

        Status = IssueStatus.Closed;
        ClosedAt = closedAt;
        ClosedByUserId = closedByUserId;
        MarkUpdated(closedByUserId, closedAt);
    }

    /// <summary>
    ///     Executes the reject domain operation.
    /// </summary>
    public void Reject(string reason, Guid rejectedByUserId, DateTimeOffset rejectedAt)
    {
        EnsureOpen();
        EnsureUserId(rejectedByUserId, nameof(rejectedByUserId));
        RejectionReason = Required(reason, nameof(reason));
        Status = IssueStatus.Rejected;
        MarkUpdated(rejectedByUserId, rejectedAt);
    }

    /// <summary>
    ///     Executes the ensure open domain operation.
    /// </summary>
    private void EnsureOpen()
    {
        if (Status is IssueStatus.Resolved or IssueStatus.Closed or IssueStatus.Rejected)
            throw new DomainException("Closed issues cannot be modified.");
    }

    /// <summary>
    ///     Executes the required domain operation.
    /// </summary>
    private static string Required(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException($"{parameterName} is required.");

        return value.Trim();
    }

    private static void EnsureUserId(Guid userId, string parameterName)
    {
        if (userId == Guid.Empty) throw new DomainException($"{parameterName} is required.");
    }
}
