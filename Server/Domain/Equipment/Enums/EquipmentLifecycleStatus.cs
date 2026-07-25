namespace Domain.Equipment.Enums;

/// <summary>
///     Defines the supported equipment lifecycle status values.
/// </summary>
public enum EquipmentLifecycleStatus
{
    /// <summary>
    ///     Represents the active state.
    /// </summary>
    Active = 1,

    /// <summary>
    ///     Represents the out of service state.
    /// </summary>
    OutOfService = 2,

    /// <summary>
    ///     Represents the decommissioned state.
    /// </summary>
    Decommissioned = 3,

    /// <summary>
    ///     Represents the archived state.
    /// </summary>
    Archived = 4
}