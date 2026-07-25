using Domain.Common;

namespace Domain.Equipment;

/// <summary>
///     Represents the equipment type domain model.
/// </summary>
public sealed class EquipmentType : AuditableEntity
{
    /// <summary>
    ///     Initializes a new instance of the EquipmentType class for persistence.
    /// </summary>
    private EquipmentType()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the EquipmentType class.
    /// </summary>
    public EquipmentType(Guid id, string name, string code, DateTimeOffset createdAt, Guid? createdByUserId = null)
        : base(id, createdAt, createdByUserId)
    {
        Name = Required(name, nameof(name));
        Code = Required(code, nameof(code));
        IsActive = true;
    }

    /// <summary>
    ///     Gets the name value.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    ///     Gets the code value.
    /// </summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>
    ///     Gets the description value.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    ///     Gets the default inspection interval months value.
    /// </summary>
    public int? DefaultInspectionIntervalMonths { get; private set; }

    /// <summary>
    ///     Gets the requires serial number value.
    /// </summary>
    public bool RequiresSerialNumber { get; private set; }

    /// <summary>
    ///     Gets the requires manufacturing year value.
    /// </summary>
    public bool RequiresManufacturingYear { get; private set; }

    /// <summary>
    ///     Gets the requires commissioning date value.
    /// </summary>
    public bool RequiresCommissioningDate { get; private set; }

    /// <summary>
    ///     Gets the is active value.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    ///     Executes the configure domain operation.
    /// </summary>
    public void Configure(
        string name,
        string code,
        string? description,
        int? defaultInspectionIntervalMonths,
        bool requiresSerialNumber,
        bool requiresManufacturingYear,
        bool requiresCommissioningDate,
        Guid? updatedByUserId,
        DateTimeOffset updatedAt)
    {
        if (defaultInspectionIntervalMonths is <= 0)
            throw new DomainException("Inspection interval must be greater than zero months.");

        Name = Required(name, nameof(name));
        Code = Required(code, nameof(code));
        Description = Optional(description);
        DefaultInspectionIntervalMonths = defaultInspectionIntervalMonths;
        RequiresSerialNumber = requiresSerialNumber;
        RequiresManufacturingYear = requiresManufacturingYear;
        RequiresCommissioningDate = requiresCommissioningDate;
        MarkUpdated(updatedByUserId, updatedAt);
    }

    /// <summary>
    ///     Executes the activate domain operation.
    /// </summary>
    public void Activate(Guid? updatedByUserId, DateTimeOffset updatedAt)
    {
        IsActive = true;
        MarkUpdated(updatedByUserId, updatedAt);
    }

    /// <summary>
    ///     Executes the deactivate domain operation.
    /// </summary>
    public void Deactivate(Guid? updatedByUserId, DateTimeOffset updatedAt)
    {
        IsActive = false;
        MarkUpdated(updatedByUserId, updatedAt);
    }

    /// <summary>
    ///     Executes the required domain operation.
    /// </summary>
    private static string Required(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException($"{parameterName} is required.");

        return value.Trim();
    }

    /// <summary>
    ///     Executes the optional domain operation.
    /// </summary>
    private static string? Optional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}