namespace Domain.Notifications.Enums;

/// <summary>
///     Defines the supported notification type values.
/// </summary>
public enum NotificationType
{
    /// <summary>
    ///     Represents the inspection due in30 days state.
    /// </summary>
    InspectionDueIn30Days = 1,

    /// <summary>
    ///     Represents the inspection due in14 days state.
    /// </summary>
    InspectionDueIn14Days = 2,

    /// <summary>
    ///     Represents the inspection due in7 days state.
    /// </summary>
    InspectionDueIn7Days = 3,

    /// <summary>
    ///     Represents the inspection due today state.
    /// </summary>
    InspectionDueToday = 4,

    /// <summary>
    ///     Represents the inspection expired state.
    /// </summary>
    InspectionExpired = 5,

    /// <summary>
    ///     Represents the issue reported state.
    /// </summary>
    IssueReported = 6,

    /// <summary>
    ///     Represents the issue assigned state.
    /// </summary>
    IssueAssigned = 7,

    /// <summary>
    ///     Represents the issue resolved state.
    /// </summary>
    IssueResolved = 8,

    /// <summary>
    ///     Represents the maintenance assigned state.
    /// </summary>
    MaintenanceAssigned = 9,

    /// <summary>
    ///     Represents the maintenance completed state.
    /// </summary>
    MaintenanceCompleted = 10,

    /// <summary>
    ///     Represents the service report completed state.
    /// </summary>
    ServiceReportCompleted = 11,

    /// <summary>
    ///     Represents the work sheet issued state.
    /// </summary>
    WorkSheetIssued = 12,

    /// <summary>
    ///     Represents the user invitation state.
    /// </summary>
    UserInvitation = 13,

    /// <summary>
    ///     Represents the custom state.
    /// </summary>
    Custom = 14
}