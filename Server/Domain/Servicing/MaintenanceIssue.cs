using Domain.Common;
using Domain.Servicing.Enums;

namespace Domain.Servicing;

/// <summary>
///     Represents the maintenance issue domain model.
/// </summary>
public sealed class MaintenanceIssue
{
    /// <summary>
    ///     Initializes a new instance of the MaintenanceIssue class for persistence.
    /// </summary>
    private MaintenanceIssue()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the MaintenanceIssue class.
    /// </summary>
    public MaintenanceIssue(Guid maintenanceId, Guid issueId, MaintenanceIssueRelationType relationType,
        DateTimeOffset createdAt, Guid? createdByUserId = null)
    {
        if (maintenanceId == Guid.Empty || issueId == Guid.Empty)
            throw new DomainException("Maintenance id and issue id are required.");

        MaintenanceId = maintenanceId;
        IssueId = issueId;
        RelationType = relationType;
        CreatedAt = createdAt;
        CreatedByUserId = createdByUserId;
    }

    /// <summary>
    ///     Gets the maintenance id value.
    /// </summary>
    public Guid MaintenanceId { get; private set; }

    /// <summary>
    ///     Gets the issue id value.
    /// </summary>
    public Guid IssueId { get; private set; }

    /// <summary>
    ///     Gets the relation type value.
    /// </summary>
    public MaintenanceIssueRelationType RelationType { get; private set; }

    /// <summary>
    ///     Gets the created at value.
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>
    ///     Gets the created by user id value.
    /// </summary>
    public Guid? CreatedByUserId { get; private set; }
}