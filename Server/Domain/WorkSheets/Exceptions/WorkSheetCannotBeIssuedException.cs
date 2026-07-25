using Domain.Common;

namespace Domain.WorkSheets.Exceptions;

/// <summary>
///     Represents a domain exception for work sheet cannot be issued.
/// </summary>
public sealed class WorkSheetCannotBeIssuedException : DomainException
{
    /// <summary>
    ///     Initializes a new instance of the WorkSheetCannotBeIssuedException class.
    /// </summary>
    public WorkSheetCannotBeIssuedException(string message)
        : base(message)
    {
    }
}