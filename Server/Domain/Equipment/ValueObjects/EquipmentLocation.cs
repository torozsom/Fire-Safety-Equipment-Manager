using Domain.Common;

namespace Domain.Equipment.ValueObjects;

/// <summary>
///     Represents the equipment location domain model.
/// </summary>
public sealed class EquipmentLocation : ValueObject
{
    /// <summary>
    ///     Initializes a new instance of the EquipmentLocation class for persistence.
    /// </summary>
    private EquipmentLocation()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the EquipmentLocation class.
    /// </summary>
    public EquipmentLocation(string? building, string? floor, string? room, string? exactLocation)
    {
        Building = Optional(building);
        Floor = Optional(floor);
        Room = Optional(room);
        ExactLocation = Optional(exactLocation);
    }

    /// <summary>
    ///     Gets the building value.
    /// </summary>
    public string? Building { get; } = string.Empty;

    /// <summary>
    ///     Gets the floor value.
    /// </summary>
    public string? Floor { get; }

    /// <summary>
    ///     Gets the room value.
    /// </summary>
    public string? Room { get; }

    /// <summary>
    ///     Gets the exact location value.
    /// </summary>
    public string? ExactLocation { get; }

    /// <summary>
    ///     Executes the get equality components domain operation.
    /// </summary>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Building;
        yield return Floor;
        yield return Room;
        yield return ExactLocation;
    }

    /// <summary>
    ///     Executes the optional domain operation.
    /// </summary>
    private static string? Optional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}