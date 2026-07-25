using Domain.Common;
using Domain.CustomerCompanies.ValueObjects;

namespace Domain.WorkSheets.ValueObjects;

/// <summary>
///     Represents the company snapshot domain model.
/// </summary>
public sealed class CompanySnapshot : ValueObject
{
    /// <summary>
    ///     Initializes a new instance of the CompanySnapshot class for persistence.
    /// </summary>
    private CompanySnapshot()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the CompanySnapshot class.
    /// </summary>
    public CompanySnapshot(string name, string? taxNumber, Address? address)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Company snapshot name is required.");

        Name = name.Trim();
        TaxNumber = string.IsNullOrWhiteSpace(taxNumber) ? null : taxNumber.Trim();
        Address = address;
    }

    /// <summary>
    ///     Gets the name value.
    /// </summary>
    public string Name { get; } = string.Empty;

    /// <summary>
    ///     Gets the tax number value.
    /// </summary>
    public string? TaxNumber { get; }

    /// <summary>
    ///     Gets the address value.
    /// </summary>
    public Address? Address { get; }

    /// <summary>
    ///     Executes the get equality components domain operation.
    /// </summary>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Name;
        yield return TaxNumber;
        yield return Address;
    }
}