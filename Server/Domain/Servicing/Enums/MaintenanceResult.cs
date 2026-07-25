namespace Domain.Servicing.Enums;

/// <summary>
///     Defines the supported maintenance result values.
/// </summary>
public enum MaintenanceResult
{
    /// <summary>
    ///     Represents the passed state.
    /// </summary>
    Passed = 1,

    /// <summary>
    ///     Represents the passed with remarks state.
    /// </summary>
    PassedWithRemarks = 2,

    /// <summary>
    ///     Represents the failed state.
    /// </summary>
    Failed = 3,

    /// <summary>
    ///     Represents the repair required state.
    /// </summary>
    RepairRequired = 4,

    /// <summary>
    ///     Represents the replacement required state.
    /// </summary>
    ReplacementRequired = 5
}