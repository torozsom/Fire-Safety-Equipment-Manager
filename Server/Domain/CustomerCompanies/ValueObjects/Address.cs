using Domain.Common;

namespace Domain.CustomerCompanies.ValueObjects;

/// <summary>
///     Represents the address domain model.
/// </summary>
public sealed class Address : ValueObject
{
    /// <summary>
    ///     Initializes a new instance of the Address class for persistence.
    /// </summary>
    private Address()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the Address class.
    /// </summary>
    public Address(string countryCode, string postalCode, string city, string addressLine, decimal? latitude = null,
        decimal? longitude = null)
    {
        CountryCode = Required(countryCode, nameof(countryCode)).ToUpperInvariant();
        PostalCode = Required(postalCode, nameof(postalCode));
        City = Required(city, nameof(city));
        AddressLine = Required(addressLine, nameof(addressLine));
        Latitude = latitude;
        Longitude = longitude;

        if (latitude is < -90 or > 90) throw new DomainException("Latitude must be between -90 and 90.");

        if (longitude is < -180 or > 180) throw new DomainException("Longitude must be between -180 and 180.");
    }

    /// <summary>
    ///     Gets the country code value.
    /// </summary>
    public string CountryCode { get; } = string.Empty;

    /// <summary>
    ///     Gets the postal code value.
    /// </summary>
    public string PostalCode { get; } = string.Empty;

    /// <summary>
    ///     Gets the city value.
    /// </summary>
    public string City { get; } = string.Empty;

    /// <summary>
    ///     Gets the address line value.
    /// </summary>
    public string AddressLine { get; } = string.Empty;

    /// <summary>
    ///     Gets the latitude value.
    /// </summary>
    public decimal? Latitude { get; }

    /// <summary>
    ///     Gets the longitude value.
    /// </summary>
    public decimal? Longitude { get; }

    /// <summary>
    ///     Executes the get equality components domain operation.
    /// </summary>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return CountryCode;
        yield return PostalCode;
        yield return City;
        yield return AddressLine;
        yield return Latitude;
        yield return Longitude;
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