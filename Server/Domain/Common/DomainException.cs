namespace Domain.Common;

/// <summary>
///     Represents a domain exception for domain.
/// </summary>
public class DomainException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the DomainException class.
    /// </summary>
    public DomainException(string message)
        : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the DomainException class.
    /// </summary>
    public DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}