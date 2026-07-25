namespace Domain.Notifications.Enums;

/// <summary>
///     Defines the supported notification status values.
/// </summary>
public enum NotificationStatus
{
    /// <summary>
    ///     Represents the pending state.
    /// </summary>
    Pending = 1,

    /// <summary>
    ///     Represents the processing state.
    /// </summary>
    Processing = 2,

    /// <summary>
    ///     Represents the sent state.
    /// </summary>
    Sent = 3,

    /// <summary>
    ///     Represents the failed state.
    /// </summary>
    Failed = 4,

    /// <summary>
    ///     Represents the cancelled state.
    /// </summary>
    Cancelled = 5,

    /// <summary>
    ///     Represents the dead lettered state.
    /// </summary>
    DeadLettered = 6
}