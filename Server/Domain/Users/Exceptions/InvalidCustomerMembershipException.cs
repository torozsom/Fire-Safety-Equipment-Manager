using Domain.Common;

namespace Domain.Users.Exceptions;

/// <summary>
///     Represents a domain exception for invalid customer membership.
/// </summary>
public sealed class InvalidCustomerMembershipException : DomainException
{
    /// <summary>
    ///     Initializes a new instance of the InvalidCustomerMembershipException class.
    /// </summary>
    public InvalidCustomerMembershipException(string message)
        : base(message)
    {
    }
}