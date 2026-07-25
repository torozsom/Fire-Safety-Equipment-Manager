namespace Domain.Users.Enums;

/// <summary>
///     Defines the supported user status values.
/// </summary>
public enum UserStatus
{
    /// <summary>
    ///     Represents the invited state.
    /// </summary>
    Invited = 1,

    /// <summary>
    ///     Represents the active state.
    /// </summary>
    Active = 2,

    /// <summary>
    ///     Represents the suspended state.
    /// </summary>
    Suspended = 3,

    /// <summary>
    ///     Represents the deactivated state.
    /// </summary>
    Deactivated = 4
}