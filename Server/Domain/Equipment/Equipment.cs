using Domain.Common;
using Domain.Equipment.Enums;
using Domain.Equipment.Events;
using Domain.Equipment.ValueObjects;

namespace Domain.Equipment;

/// <summary>
///     Represents the equipment domain model.
/// </summary>
public sealed class Equipment : AuditableEntity
{
    /// <summary>
    ///     Initializes a new instance of the Equipment class for persistence.
    /// </summary>
    private Equipment()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the Equipment class.
    /// </summary>
    public Equipment(
        Guid id,
        Guid siteId,
        Guid equipmentTypeId,
        AssetIdentifier assetIdentifier,
        DateTimeOffset registeredAt,
        Guid? registeredByUserId = null)
        : base(id, registeredAt, registeredByUserId)
    {
        if (siteId == Guid.Empty) throw new DomainException("Site id is required.");

        if (equipmentTypeId == Guid.Empty) throw new DomainException("Equipment type id is required.");

        SiteId = siteId;
        EquipmentTypeId = equipmentTypeId;
        AssetIdentifier = assetIdentifier;
        PublicToken = Guid.NewGuid().ToString("N");
        LifecycleStatus = EquipmentLifecycleStatus.Active;
        AddDomainEvent(new EquipmentRegisteredDomainEvent(Id, SiteId, registeredAt));
    }

    /// <summary>
    ///     Gets the site id value.
    /// </summary>
    public Guid SiteId { get; }

    /// <summary>
    ///     Gets the equipment type id value.
    /// </summary>
    public Guid EquipmentTypeId { get; private set; }

    /// <summary>
    ///     Gets the asset identifier value.
    /// </summary>
    public AssetIdentifier AssetIdentifier { get; private set; } = null!;

    /// <summary>
    ///     Gets the public token value.
    /// </summary>
    public string PublicToken { get; private set; } = string.Empty;

    /// <summary>
    ///     Gets the serial number value.
    /// </summary>
    public string? SerialNumber { get; private set; }

    /// <summary>
    ///     Gets the manufacturer value.
    /// </summary>
    public string? Manufacturer { get; private set; }

    /// <summary>
    ///     Gets the model value.
    /// </summary>
    public string? Model { get; private set; }

    /// <summary>
    ///     Gets the manufacturing year value.
    /// </summary>
    public int? ManufacturingYear { get; private set; }

    /// <summary>
    ///     Gets the commissioned on value.
    /// </summary>
    public DateOnly? CommissionedOn { get; private set; }

    /// <summary>
    ///     Gets the location value.
    /// </summary>
    public EquipmentLocation? Location { get; private set; }

    /// <summary>
    ///     Gets the inspection interval months value.
    /// </summary>
    public int? InspectionIntervalMonths { get; private set; }

    /// <summary>
    ///     Gets the last inspection date value.
    /// </summary>
    public DateOnly? LastInspectionDate { get; private set; }

    /// <summary>
    ///     Gets the next inspection due date value.
    /// </summary>
    public DateOnly? NextInspectionDueDate { get; private set; }

    /// <summary>
    ///     Gets the lifecycle status value.
    /// </summary>
    public EquipmentLifecycleStatus LifecycleStatus { get; private set; }

    /// <summary>
    ///     Gets the out of service reason value.
    /// </summary>
    public string? OutOfServiceReason { get; private set; }

    /// <summary>
    ///     Gets the decommissioned on value.
    /// </summary>
    public DateOnly? DecommissionedOn { get; private set; }

    /// <summary>
    ///     Gets the notes value.
    /// </summary>
    public string? Notes { get; private set; }

    /// <summary>
    ///     Gets the archived at value.
    /// </summary>
    public DateTimeOffset? ArchivedAt { get; private set; }

    /// <summary>
    ///     Gets the archived by user id value.
    /// </summary>
    public Guid? ArchivedByUserId { get; private set; }

    /// <summary>
    ///     Executes the update technical data domain operation.
    /// </summary>
    public void UpdateTechnicalData(
        string? serialNumber,
        string? manufacturer,
        string? model,
        int? manufacturingYear,
        DateOnly? commissionedOn,
        EquipmentLocation? location,
        string? notes,
        Guid? updatedByUserId,
        DateTimeOffset updatedAt)
    {
        EnsureActiveRecord();
        if (manufacturingYear.HasValue &&
            (manufacturingYear.Value < 1900 || manufacturingYear.Value > DateTime.UtcNow.Year))
            throw new DomainException("Manufacturing year cannot be in the future.");

        SerialNumber = Optional(serialNumber);
        Manufacturer = Optional(manufacturer);
        Model = Optional(model);
        ManufacturingYear = manufacturingYear;
        CommissionedOn = commissionedOn;
        Location = location;
        Notes = Optional(notes);
        MarkUpdated(updatedByUserId, updatedAt);
    }

    /// <summary>
    ///     Executes the set inspection schedule domain operation.
    /// </summary>
    public void SetInspectionSchedule(int? inspectionIntervalMonths, DateOnly? lastInspectionDate,
        DateOnly? nextInspectionDueDate, Guid? updatedByUserId, DateTimeOffset updatedAt)
    {
        if (inspectionIntervalMonths is <= 0)
            throw new DomainException("Inspection interval must be greater than zero months.");

        if (lastInspectionDate.HasValue && nextInspectionDueDate.HasValue &&
            nextInspectionDueDate <= lastInspectionDate)
            throw new DomainException("Next inspection due date must be after the last inspection date.");

        InspectionIntervalMonths = inspectionIntervalMonths;
        LastInspectionDate = lastInspectionDate;
        NextInspectionDueDate = nextInspectionDueDate;
        MarkUpdated(updatedByUserId, updatedAt);
    }

    /// <summary>
    ///     Executes the mark inspection expired domain operation.
    /// </summary>
    public void MarkInspectionExpired(DateTimeOffset detectedAt)
    {
        if (NextInspectionDueDate.HasValue)
            AddDomainEvent(new EquipmentInspectionExpiredDomainEvent(Id, NextInspectionDueDate.Value, detectedAt));
    }

    /// <summary>
    ///     Executes the put out of service domain operation.
    /// </summary>
    public void PutOutOfService(string reason, Guid? updatedByUserId, DateTimeOffset updatedAt)
    {
        EnsureActiveRecord();
        LifecycleStatus = EquipmentLifecycleStatus.OutOfService;
        OutOfServiceReason = Required(reason, nameof(reason));
        MarkUpdated(updatedByUserId, updatedAt);
    }

    /// <summary>
    ///     Executes the return to service domain operation.
    /// </summary>
    public void ReturnToService(Guid? updatedByUserId, DateTimeOffset updatedAt)
    {
        if (LifecycleStatus != EquipmentLifecycleStatus.OutOfService)
            throw new DomainException("Only out-of-service equipment can be returned to service.");

        LifecycleStatus = EquipmentLifecycleStatus.Active;
        OutOfServiceReason = null;
        MarkUpdated(updatedByUserId, updatedAt);
    }

    /// <summary>
    ///     Executes the decommission domain operation.
    /// </summary>
    public void Decommission(DateOnly decommissionedOn, Guid? updatedByUserId, DateTimeOffset updatedAt)
    {
        EnsureActiveRecord();
        LifecycleStatus = EquipmentLifecycleStatus.Decommissioned;
        DecommissionedOn = decommissionedOn;
        MarkUpdated(updatedByUserId, updatedAt);
    }

    /// <summary>
    ///     Executes the archive domain operation.
    /// </summary>
    public void Archive(Guid? archivedByUserId, DateTimeOffset archivedAt)
    {
        LifecycleStatus = EquipmentLifecycleStatus.Archived;
        ArchivedAt = archivedAt;
        ArchivedByUserId = archivedByUserId;
        MarkUpdated(archivedByUserId, archivedAt);
    }

    /// <summary>
    ///     Executes the ensure active record domain operation.
    /// </summary>
    private void EnsureActiveRecord()
    {
        if (LifecycleStatus == EquipmentLifecycleStatus.Archived)
            throw new DomainException("Archived equipment cannot be modified.");
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
