using Domain.Common;

namespace Domain.Servicing.Exceptions;

/// <summary>
///     Represents a domain exception for invalid maintenance state.
/// </summary>
public sealed class InvalidMaintenanceStateException : DomainException
{
    /// <summary>
    ///     Initializes a new instance of the InvalidMaintenanceStateException class.
    /// </summary>
    public InvalidMaintenanceStateException(string message)
        : base(message)
    {
    }
}