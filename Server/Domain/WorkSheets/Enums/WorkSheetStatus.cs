namespace Domain.WorkSheets.Enums;

/// <summary>
///     Defines the supported work sheet status values.
/// </summary>
public enum WorkSheetStatus
{
    /// <summary>
    ///     Represents the draft state.
    /// </summary>
    Draft = 1,

    /// <summary>
    ///     Represents the issued state.
    /// </summary>
    Issued = 2,

    /// <summary>
    ///     Represents the accepted state.
    /// </summary>
    Accepted = 3,

    /// <summary>
    ///     Represents the superseded state.
    /// </summary>
    Superseded = 4,

    /// <summary>
    ///     Represents the cancelled state.
    /// </summary>
    Cancelled = 5
}