namespace Domain.Servicing.Enums;

/// <summary>
///     Defines the supported maintenance issue relation type values.
/// </summary>
public enum MaintenanceIssueRelationType
{
    /// <summary>
    ///     Represents the detected state.
    /// </summary>
    Detected = 1,

    /// <summary>
    ///     Represents the resolved state.
    /// </summary>
    Resolved = 2,

    /// <summary>
    ///     Represents the related state.
    /// </summary>
    Related = 3
}