namespace Domain.Users.Enums;

/// <summary>
///     Defines the supported customer membership role values.
/// </summary>
public enum CustomerMembershipRole
{
    /// <summary>
    ///     Represents the customer state.
    /// </summary>
    Customer = 1,

    /// <summary>
    ///     Represents the maintenance technician state.
    /// </summary>
    MaintenanceTechnician = 2,

    /// <summary>
    ///     Represents the company administrator state.
    /// </summary>
    CompanyAdministrator = 3
}