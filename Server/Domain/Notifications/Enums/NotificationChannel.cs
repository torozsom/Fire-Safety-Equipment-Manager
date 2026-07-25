namespace Domain.Notifications.Enums;

/// <summary>
///     Defines the supported notification channel values.
/// </summary>
public enum NotificationChannel
{
    /// <summary>
    ///     Represents the email state.
    /// </summary>
    Email = 1,

    /// <summary>
    ///     Represents the in app state.
    /// </summary>
    InApp = 2,

    /// <summary>
    ///     Represents the sms state.
    /// </summary>
    Sms = 3
}