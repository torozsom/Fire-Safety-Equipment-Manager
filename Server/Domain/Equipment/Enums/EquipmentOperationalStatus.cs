namespace Domain.Equipment.Enums;

/// <summary>
///     Defines the supported equipment operational status values.
/// </summary>
public enum EquipmentOperationalStatus
{
    /// <summary>
    ///     Represents the ok state.
    /// </summary>
    Ok = 1,

    /// <summary>
    ///     Represents the warning state.
    /// </summary>
    Warning = 2,

    /// <summary>
    ///     Represents the expired state.
    /// </summary>
    Expired = 3,

    /// <summary>
    ///     Represents the faulty state.
    /// </summary>
    Faulty = 4,

    /// <summary>
    ///     Represents the out of service state.
    /// </summary>
    OutOfService = 5,

    /// <summary>
    ///     Represents the decommissioned state.
    /// </summary>
    Decommissioned = 6
}