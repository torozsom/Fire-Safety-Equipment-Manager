namespace Domain.Servicing.Enums;

/// <summary>
///     Defines the supported issue status values.
/// </summary>
public enum IssueStatus
{
    /// <summary>
    ///     Represents the open state.
    /// </summary>
    Open = 1,

    /// <summary>
    ///     Represents the acknowledged state.
    /// </summary>
    Acknowledged = 2,

    /// <summary>
    ///     Represents the in progress state.
    /// </summary>
    InProgress = 3,

    /// <summary>
    ///     Represents the resolved state.
    /// </summary>
    Resolved = 4,

    /// <summary>
    ///     Represents the closed state.
    /// </summary>
    Closed = 5,

    /// <summary>
    ///     Represents the rejected state.
    /// </summary>
    Rejected = 6
}