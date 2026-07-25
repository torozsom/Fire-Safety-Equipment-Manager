namespace Domain.Servicing.Enums;

/// <summary>
///     Defines the supported service report status values.
/// </summary>
public enum ServiceReportStatus
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