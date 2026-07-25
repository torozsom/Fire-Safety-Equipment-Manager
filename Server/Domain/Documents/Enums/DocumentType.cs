namespace Domain.Documents.Enums;

/// <summary>
///     Defines the supported document type values.
/// </summary>
public enum DocumentType
{
    /// <summary>
    ///     Represents the equipment photo state.
    /// </summary>
    EquipmentPhoto = 1,

    /// <summary>
    ///     Represents the service report photo state.
    /// </summary>
    ServiceReportPhoto = 2,

    /// <summary>
    ///     Represents the maintenance photo state.
    /// </summary>
    MaintenancePhoto = 3,

    /// <summary>
    ///     Represents the issue photo state.
    /// </summary>
    IssuePhoto = 4,

    /// <summary>
    ///     Represents the work sheet pdf state.
    /// </summary>
    WorkSheetPdf = 5,

    /// <summary>
    ///     Represents the work sheet attachment state.
    /// </summary>
    WorkSheetAttachment = 6,

    /// <summary>
    ///     Represents the customer signature state.
    /// </summary>
    CustomerSignature = 7,

    /// <summary>
    ///     Represents the inspection report state.
    /// </summary>
    InspectionReport = 8,

    /// <summary>
    ///     Represents the certificate state.
    /// </summary>
    Certificate = 9,

    /// <summary>
    ///     Represents the manufacturer document state.
    /// </summary>
    ManufacturerDocument = 10,

    /// <summary>
    ///     Represents the other state.
    /// </summary>
    Other = 11
}