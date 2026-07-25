using Domain.Common;

namespace Domain.Documents.ValueObjects;

/// <summary>
///     Represents the file hash domain model.
/// </summary>
public sealed class FileHash : ValueObject
{
    /// <summary>
    ///     Initializes a new instance of the FileHash class for persistence.
    /// </summary>
    private FileHash()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the FileHash class.
    /// </summary>
    public FileHash(string sha256Hash)
    {
        if (string.IsNullOrWhiteSpace(sha256Hash) || sha256Hash.Trim().Length != 64 ||
            !sha256Hash.Trim().All(Uri.IsHexDigit))
            throw new DomainException("SHA-256 hash must be a 64-character hexadecimal value.");

        Sha256Hash = sha256Hash.Trim().ToLowerInvariant();
    }

    /// <summary>
    ///     Gets the sha256 hash value.
    /// </summary>
    public string Sha256Hash { get; } = string.Empty;

    /// <summary>
    ///     Executes the get equality components domain operation.
    /// </summary>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Sha256Hash;
    }
}