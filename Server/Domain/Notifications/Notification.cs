using Domain.Common;
using Domain.Notifications.Enums;
using Domain.Notifications.ValueObjects;

namespace Domain.Notifications;

/// <summary>
///     Represents the notification domain model.
/// </summary>
public sealed class Notification : AuditableEntity
{
    /// <summary>
    ///     Initializes a new instance of the Notification class for persistence.
    /// </summary>
    private Notification()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the Notification class.
    /// </summary>
    public Notification(
        Guid id,
        NotificationType type,
        NotificationChannel channel,
        string recipientEmail,
        string subject,
        string body,
        DeduplicationKey deduplicationKey,
        DateTimeOffset scheduledAt,
        DateTimeOffset createdAt,
        Guid customerCompanyId,
        Guid? recipientUserId = null)
        : base(id, createdAt, null)
    {
        Type = type;
        Channel = channel;
        RecipientEmail = Required(recipientEmail, nameof(recipientEmail));
        Subject = Required(subject, nameof(subject));
        Body = Required(body, nameof(body));
        DeduplicationKey = deduplicationKey;
        ScheduledAt = scheduledAt;
        RecipientUserId = recipientUserId;
        CustomerCompanyId = customerCompanyId == Guid.Empty
            ? throw new DomainException("Customer company id is required.")
            : customerCompanyId;
        Status = NotificationStatus.Pending;
    }

    /// <summary>
    ///     Gets the type value.
    /// </summary>
    public NotificationType Type { get; private set; }

    /// <summary>
    ///     Gets the channel value.
    /// </summary>
    public NotificationChannel Channel { get; private set; }

    /// <summary>
    ///     Gets the status value.
    /// </summary>
    public NotificationStatus Status { get; private set; }

    /// <summary>
    ///     Gets the recipient user id value.
    /// </summary>
    public Guid? RecipientUserId { get; private set; }

    /// <summary>
    ///     Gets the recipient email value.
    /// </summary>
    public string RecipientEmail { get; private set; } = string.Empty;

    /// <summary>
    ///     Gets the customer company id value.
    /// </summary>
    public Guid CustomerCompanyId { get; private set; }

    /// <summary>
    ///     Gets the equipment id value.
    /// </summary>
    public Guid? EquipmentId { get; private set; }

    /// <summary>
    ///     Gets the service report id value.
    /// </summary>
    public Guid? ServiceReportId { get; private set; }

    /// <summary>
    ///     Gets the maintenance id value.
    /// </summary>
    public Guid? MaintenanceId { get; private set; }

    /// <summary>
    ///     Gets the issue id value.
    /// </summary>
    public Guid? IssueId { get; private set; }

    /// <summary>
    ///     Gets the work sheet id value.
    /// </summary>
    public Guid? WorkSheetId { get; private set; }

    /// <summary>
    ///     Gets the subject value.
    /// </summary>
    public string Subject { get; private set; } = string.Empty;

    /// <summary>
    ///     Gets the body value.
    /// </summary>
    public string Body { get; private set; } = string.Empty;

    /// <summary>
    ///     Gets the scheduled at value.
    /// </summary>
    public DateTimeOffset ScheduledAt { get; private set; }

    /// <summary>
    ///     Gets the sent at value.
    /// </summary>
    public DateTimeOffset? SentAt { get; private set; }

    /// <summary>
    ///     Gets the failed at value.
    /// </summary>
    public DateTimeOffset? FailedAt { get; private set; }

    /// <summary>
    ///     Gets the attempt count value.
    /// </summary>
    public int AttemptCount { get; private set; }

    /// <summary>
    ///     Gets the next attempt at value.
    /// </summary>
    public DateTimeOffset? NextAttemptAt { get; private set; }

    /// <summary>
    ///     Gets the last error value.
    /// </summary>
    public string? LastError { get; private set; }

    /// <summary>
    ///     Gets the external message id value.
    /// </summary>
    public string? ExternalMessageId { get; private set; }

    /// <summary>
    ///     Gets the deduplication key value.
    /// </summary>
    public DeduplicationKey DeduplicationKey { get; private set; } = null!;

    /// <summary>
    ///     Gets the cancelled at value.
    /// </summary>
    public DateTimeOffset? CancelledAt { get; private set; }

    /// <summary>
    ///     Gets the cancelled by user id value.
    /// </summary>
    public Guid? CancelledByUserId { get; private set; }

    /// <summary>
    ///     Executes the attach context domain operation.
    /// </summary>
    public void AttachContext(Guid? equipmentId, Guid? serviceReportId, Guid? maintenanceId, Guid? issueId,
        Guid? workSheetId)
    {
        EquipmentId = equipmentId;
        ServiceReportId = serviceReportId;
        MaintenanceId = maintenanceId;
        IssueId = issueId;
        WorkSheetId = workSheetId;
    }

    /// <summary>
    ///     Executes the mark processing domain operation.
    /// </summary>
    public void MarkProcessing(DateTimeOffset processingAt)
    {
        if (Status != NotificationStatus.Pending && Status != NotificationStatus.Failed)
            throw new DomainException("Only pending or failed notifications can be processed.");

        Status = NotificationStatus.Processing;
        LastError = null;
        MarkUpdated(null, processingAt);
    }

    /// <summary>
    ///     Executes the mark sent domain operation.
    /// </summary>
    public void MarkSent(string? externalMessageId, DateTimeOffset sentAt)
    {
        if (Status != NotificationStatus.Processing)
            throw new DomainException("Only processing notifications can be sent.");

        Status = NotificationStatus.Sent;
        SentAt = sentAt;
        ExternalMessageId = string.IsNullOrWhiteSpace(externalMessageId) ? null : externalMessageId.Trim();
        MarkUpdated(null, sentAt);
    }

    /// <summary>
    ///     Executes the mark failed domain operation.
    /// </summary>
    public void MarkFailed(string error, DateTimeOffset failedAt, DateTimeOffset? nextAttemptAt, int maxAttempts)
    {
        if (Status != NotificationStatus.Processing)
            throw new DomainException("Only processing notifications can fail.");
        if (maxAttempts <= 0)
            throw new DomainException("Maximum attempts must be greater than zero.");
        AttemptCount++;
        FailedAt = failedAt;
        LastError = Required(error, nameof(error));
        NextAttemptAt = nextAttemptAt;
        Status = AttemptCount >= maxAttempts ? NotificationStatus.DeadLettered : NotificationStatus.Failed;
        MarkUpdated(null, failedAt);
    }

    /// <summary>
    ///     Executes the cancel domain operation.
    /// </summary>
    public void Cancel(Guid? cancelledByUserId, DateTimeOffset cancelledAt)
    {
        if (Status == NotificationStatus.Sent) throw new DomainException("Sent notifications cannot be cancelled.");

        Status = NotificationStatus.Cancelled;
        CancelledAt = cancelledAt;
        CancelledByUserId = cancelledByUserId;
        MarkUpdated(cancelledByUserId, cancelledAt);
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
