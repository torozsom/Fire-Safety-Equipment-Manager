namespace Domain.Servicing.Enums;

/// <summary>
///     Defines the supported maintenance type values.
/// </summary>
public enum MaintenanceType
{
    /// <summary>
    ///     Represents the inspection state.
    /// </summary>
    Inspection = 1,

    /// <summary>
    ///     Represents the preventive maintenance state.
    /// </summary>
    PreventiveMaintenance = 2,

    /// <summary>
    ///     Represents the repair state.
    /// </summary>
    Repair = 3,

    /// <summary>
    ///     Represents the replacement state.
    /// </summary>
    Replacement = 4,

    /// <summary>
    ///     Represents the commissioning state.
    /// </summary>
    Commissioning = 5,

    /// <summary>
    ///     Represents the decommissioning state.
    /// </summary>
    Decommissioning = 6
}