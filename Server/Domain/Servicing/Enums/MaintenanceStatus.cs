namespace Domain.Servicing.Enums;

/// <summary>
///     Defines the supported maintenance status values.
/// </summary>
public enum MaintenanceStatus
{
    /// <summary>
    ///     Represents the planned state.
    /// </summary>
    Planned = 1,

    /// <summary>
    ///     Represents the in progress state.
    /// </summary>
    InProgress = 2,

    /// <summary>
    ///     Represents the completed state.
    /// </summary>
    Completed = 3,

    /// <summary>
    ///     Represents the cancelled state.
    /// </summary>
    Cancelled = 4
}