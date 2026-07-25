using Domain.Common;
using Domain.CustomerCompanies.ValueObjects;

namespace Domain.WorkSheets.ValueObjects;

/// <summary>
///     Represents the site snapshot domain model.
/// </summary>
public sealed class SiteSnapshot : ValueObject
{
    /// <summary>
    ///     Initializes a new instance of the SiteSnapshot class for persistence.
    /// </summary>
    private SiteSnapshot()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the SiteSnapshot class.
    /// </summary>
    public SiteSnapshot(string name, Address address)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Site snapshot name is required.");

        Name = name.Trim();
        Address = address;
    }

    /// <summary>
    ///     Gets the name value.
    /// </summary>
    public string Name { get; } = string.Empty;

    /// <summary>
    ///     Gets the address value.
    /// </summary>
    public Address Address { get; } = null!;

    /// <summary>
    ///     Executes the get equality components domain operation.
    /// </summary>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Name;
        yield return Address;
    }
}