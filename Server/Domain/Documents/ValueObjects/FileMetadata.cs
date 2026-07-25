using Domain.Common;

namespace Domain.Documents.ValueObjects;

/// <summary>
///     Represents the file metadata domain model.
/// </summary>
public sealed class FileMetadata : ValueObject
{
    /// <summary>
    ///     Initializes a new instance of the FileMetadata class for persistence.
    /// </summary>
    private FileMetadata()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the FileMetadata class.
    /// </summary>
    public FileMetadata(string originalFileName, string storageKey, string contentType, string fileExtension,
        long sizeInBytes)
    {
        if (sizeInBytes <= 0) throw new DomainException("File size must be greater than zero.");

        OriginalFileName = Required(originalFileName, nameof(originalFileName));
        StorageKey = Required(storageKey, nameof(storageKey));
        ContentType = Required(contentType, nameof(contentType));
        FileExtension = Required(fileExtension, nameof(fileExtension)).TrimStart('.').ToLowerInvariant();
        SizeInBytes = sizeInBytes;
    }

    /// <summary>
    ///     Gets the original file name value.
    /// </summary>
    public string OriginalFileName { get; } = string.Empty;

    /// <summary>
    ///     Gets the storage key value.
    /// </summary>
    public string StorageKey { get; } = string.Empty;

    /// <summary>
    ///     Gets the content type value.
    /// </summary>
    public string ContentType { get; } = string.Empty;

    /// <summary>
    ///     Gets the file extension value.
    /// </summary>
    public string FileExtension { get; } = string.Empty;

    /// <summary>
    ///     Gets the size in bytes value.
    /// </summary>
    public long SizeInBytes { get; }

    /// <summary>
    ///     Executes the get equality components domain operation.
    /// </summary>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return OriginalFileName;
        yield return StorageKey;
        yield return ContentType;
        yield return FileExtension;
        yield return SizeInBytes;
    }

    /// <summary>
    ///     Executes the required domain operation.
    /// </summary>
    private static string Required(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException($"{parameterName} is required.");

        return value.Trim();
    }
}