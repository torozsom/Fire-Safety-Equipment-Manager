using Domain.Common;
using Domain.Servicing.Enums;
using Domain.Servicing.Events;
using Domain.Servicing.Exceptions;

namespace Domain.Servicing;

/// <summary>
///     Represents the maintenance domain model.
/// </summary>
public sealed class Maintenance : AuditableEntity
{
    /// <summary>
    ///     Initializes a new instance of the Maintenance class for persistence.
    /// </summary>
    private Maintenance()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the Maintenance class.
    /// </summary>
    public Maintenance(Guid id, Guid serviceReportId, Guid equipmentId, MaintenanceType type, DateTimeOffset createdAt,
        Guid? createdByUserId = null)
        : base(id, createdAt, createdByUserId)
    {
        if (serviceReportId == Guid.Empty || equipmentId == Guid.Empty)
            throw new InvalidMaintenanceStateException("Service report id and equipment id are required.");

        ServiceReportId = serviceReportId;
        EquipmentId = equipmentId;
        Type = type;
        Status = MaintenanceStatus.Planned;
    }

    /// <summary>
    ///     Gets the service report id value.
    /// </summary>
    public Guid ServiceReportId { get; private set; }

    /// <summary>
    ///     Gets the equipment id value.
    /// </summary>
    public Guid EquipmentId { get; }

    /// <summary>
    ///     Gets the type value.
    /// </summary>
    public MaintenanceType Type { get; private set; }

    /// <summary>
    ///     Gets the status value.
    /// </summary>
    public MaintenanceStatus Status { get; private set; }

    /// <summary>
    ///     Gets the scheduled date value.
    /// </summary>
    public DateOnly? ScheduledDate { get; private set; }

    /// <summary>
    ///     Gets the started at value.
    /// </summary>
    public DateTimeOffset? StartedAt { get; private set; }

    /// <summary>
    ///     Gets the performed date value.
    /// </summary>
    public DateOnly? PerformedDate { get; private set; }

    /// <summary>
    ///     Gets the assigned technician user id value.
    /// </summary>
    public Guid? AssignedTechnicianUserId { get; private set; }

    /// <summary>
    ///     Gets the performed by user id value.
    /// </summary>
    public Guid? PerformedByUserId { get; private set; }

    /// <summary>
    ///     Gets the result value.
    /// </summary>
    public MaintenanceResult? Result { get; private set; }

    /// <summary>
    ///     Gets the work description value.
    /// </summary>
    public string? WorkDescription { get; private set; }

    /// <summary>
    ///     Gets the findings value.
    /// </summary>
    public string? Findings { get; private set; }

    /// <summary>
    ///     Gets the recommendation value.
    /// </summary>
    public string? Recommendation { get; private set; }

    /// <summary>
    ///     Gets the next inspection due date value.
    /// </summary>
    public DateOnly? NextInspectionDueDate { get; private set; }

    /// <summary>
    ///     Gets the cancellation reason value.
    /// </summary>
    public string? CancellationReason { get; private set; }

    /// <summary>
    ///     Gets the completed at value.
    /// </summary>
    public DateTimeOffset? CompletedAt { get; private set; }

    /// <summary>
    ///     Gets the completed by user id value.
    /// </summary>
    public Guid? CompletedByUserId { get; private set; }

    /// <summary>
    ///     Executes the schedule domain operation.
    /// </summary>
    public void Schedule(DateOnly? scheduledDate, Guid? assignedTechnicianUserId, Guid? updatedByUserId,
        DateTimeOffset updatedAt)
    {
        EnsureNotClosed();
        ScheduledDate = scheduledDate;
        AssignedTechnicianUserId = assignedTechnicianUserId;
        MarkUpdated(updatedByUserId, updatedAt);
    }

    /// <summary>
    ///     Executes the start domain operation.
    /// </summary>
    public void Start(DateTimeOffset startedAt, Guid? startedByUserId)
    {
        if (Status != MaintenanceStatus.Planned)
            throw new InvalidMaintenanceStateException("Only planned maintenance can be started.");

        Status = MaintenanceStatus.InProgress;
        StartedAt = startedAt;
        MarkUpdated(startedByUserId, startedAt);
    }

    /// <summary>
    ///     Executes the complete domain operation.
    /// </summary>
    public void Complete(
        DateOnly performedDate,
        Guid performedByUserId,
        MaintenanceResult result,
        string workDescription,
        string findings,
        string? recommendation,
        DateOnly? nextInspectionDueDate,
        Guid? completedByUserId,
        DateTimeOffset completedAt)
    {
        if (Status is MaintenanceStatus.Completed or MaintenanceStatus.Cancelled)
            throw new InvalidMaintenanceStateException("Closed maintenance cannot be completed.");

        if (performedByUserId == Guid.Empty)
            throw new InvalidMaintenanceStateException("Performed by user id is required.");

        PerformedDate = performedDate;
        PerformedByUserId = performedByUserId;
        Result = result;
        WorkDescription = Required(workDescription, nameof(workDescription));
        Findings = Required(findings, nameof(findings));
        Recommendation = Optional(recommendation);
        NextInspectionDueDate = nextInspectionDueDate;
        Status = MaintenanceStatus.Completed;
        CompletedAt = completedAt;
        CompletedByUserId = completedByUserId;
        MarkUpdated(completedByUserId, completedAt);
        AddDomainEvent(new MaintenanceCompletedDomainEvent(Id, EquipmentId, completedAt));
    }

    /// <summary>
    ///     Executes the cancel domain operation.
    /// </summary>
    public void Cancel(string reason, Guid? cancelledByUserId, DateTimeOffset cancelledAt)
    {
        EnsureNotClosed();
        CancellationReason = Required(reason, nameof(reason));
        Status = MaintenanceStatus.Cancelled;
        MarkUpdated(cancelledByUserId, cancelledAt);
    }

    /// <summary>
    ///     Executes the ensure not closed domain operation.
    /// </summary>
    private void EnsureNotClosed()
    {
        if (Status is MaintenanceStatus.Completed or MaintenanceStatus.Cancelled)
            throw new InvalidMaintenanceStateException("Closed maintenance cannot be modified.");
    }

    /// <summary>
    ///     Executes the required domain operation.
    /// </summary>
    private static string Required(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidMaintenanceStateException($"{parameterName} is required.");

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