using Domain.Common;

namespace Domain.Servicing.Exceptions;

/// <summary>
///     Represents a domain exception for invalid service report state.
/// </summary>
public sealed class InvalidServiceReportStateException : DomainException
{
    /// <summary>
    ///     Initializes a new instance of the InvalidServiceReportStateException class.
    /// </summary>
    public InvalidServiceReportStateException(string message)
        : base(message)
    {
    }
}