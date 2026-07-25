using Domain.Equipment.Enums;
using Domain.Servicing;
using Domain.Servicing.Enums;

namespace Domain.Equipment.Services;

/// <summary>
///     Represents the equipment status calculator domain model.
/// </summary>
public sealed class EquipmentStatusCalculator
{
    /// <summary>
    ///     Executes the calculate domain operation.
    /// </summary>
    public EquipmentOperationalStatus Calculate(Equipment equipment, IEnumerable<Issue> issues, DateOnly today)
    {
        if (equipment.LifecycleStatus == EquipmentLifecycleStatus.Decommissioned)
            return EquipmentOperationalStatus.Decommissioned;

        if (equipment.LifecycleStatus == EquipmentLifecycleStatus.OutOfService)
            return EquipmentOperationalStatus.OutOfService;

        if (issues.Any(issue =>
                issue.Status is not IssueStatus.Resolved and not IssueStatus.Closed and not IssueStatus.Rejected
                && issue.Severity is IssueSeverity.High or IssueSeverity.Critical))
            return EquipmentOperationalStatus.Faulty;

        if (equipment.NextInspectionDueDate.HasValue && equipment.NextInspectionDueDate.Value < today)
            return EquipmentOperationalStatus.Expired;

        if (equipment.NextInspectionDueDate.HasValue && equipment.NextInspectionDueDate.Value <= today.AddDays(30))
            return EquipmentOperationalStatus.Warning;

        return EquipmentOperationalStatus.Ok;
    }
}