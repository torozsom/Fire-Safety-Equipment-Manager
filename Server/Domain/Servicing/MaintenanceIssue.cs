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
    public MaintenanceIssue(Maintenance maintenance, Issue issue, MaintenanceIssueRelationType relationType,
        DateTimeOffset createdAt, Guid? createdByUserId = null)
    {
        ArgumentNullException.ThrowIfNull(maintenance);
        ArgumentNullException.ThrowIfNull(issue);

        if (maintenance.EquipmentId != issue.EquipmentId)
            throw new DomainException("Maintenance and issue must belong to the same equipment.");

        MaintenanceId = maintenance.Id;
        IssueId = issue.Id;
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
