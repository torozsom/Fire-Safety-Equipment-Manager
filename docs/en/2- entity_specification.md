# Fire Safety Equipment Management System
## Entity and Relational Model Developer Specification

**Document type:** developer data model specification  
**Language:** English  
**Format:** Markdown  
**Scope:** the system’s approved domain entities, their fields, relationships, constraints, and principal business rules

---

# 1. Purpose and Scope

This document defines the proposed relational data model for the Fire Safety Equipment Management System.

Its purpose is to provide a consistent foundation for:

- designing the database schema;
- implementing the domain and persistence layers of the ASP.NET Core backend;
- designing API resources at a later stage;
- implementing authorization;
- implementing maintenance, issue-management, and documentation workflows.

The specification covers the following entities:

1. `User`
2. `CustomerCompany`
3. `CustomerMembership`
4. `Site`
5. `EquipmentType`
6. `Equipment`
7. `ServiceReport`
8. `Maintenance`
9. `MaintenanceIssue`
10. `Issue`
11. `WorkSheet`
12. `Document`
13. `Notification`

---

# 2. General Modeling Principles

## 2.1. Primary Keys

Every principal entity has its own technical primary key.

| Property | Recommendation |
|---|---|
| Key name | `Id` |
| Type | UUID / GUID |
| Generation | Application-side or database-side |
| Visibility | Internal technical identifier |

Human-readable identifiers—such as equipment identifiers, issue numbers, and worksheet numbers—are stored in separate fields.

## 2.2. Timestamps and Dates

| Data type | Usage |
|---|---|
| Timestamp with time-zone information | Creation, update, sending, completion, login |
| Calendar date | Inspection date, maintenance date, commissioning date |
| Year value | Manufacturing year |

## 2.3. Audit Fields

The following fields are recommended for most business entities:

- `CreatedAt`
- `CreatedByUserId`
- `UpdatedAt`
- `UpdatedByUserId`

For deactivation, cancellation, or archiving:

- `ArchivedAt`
- `ArchivedByUserId`
- `DeactivatedAt`
- `DeactivatedByUserId`
- `CancelledAt`
- `CancelledByUserId`

## 2.4. Physical Deletion

Historical or legally relevant records must not be physically deleted.

Archiving, deactivation, invalidation, or soft deletion should be used for:

- users;
- customer companies;
- sites;
- equipment;
- maintenance records;
- issues;
- worksheets;
- documents;
- notification logs.

## 2.5. Concurrency Control

Optimistic concurrency control is recommended for mutable business records.

| Field | Purpose |
|---|---|
| `RowVersion` | Detects whether another process has modified the record in the meantime |

## 2.6. Authorization Model

Authorization has two levels:

1. system-level authorization;
2. customer-company-specific membership role.

The `User` has an optional system-level role. The customer-company-specific role is stored in the `CustomerMembership` record.

---

# 3. User

## 3.1. Business Meaning

The `User` represents a natural person who can sign in to the system.

A user may be associated with multiple customer companies, and the same user may have a different role at each company.

## 3.2. Fields

| Field | Type | Required | Unique | Description |
|---|---|---:|---:|---|
| `Id` | UUID / GUID | yes | yes | Primary key |
| `Email` | text | yes | yes | Login and contact email address |
| `NormalizedEmail` | text | yes | yes | Normalized email for searching and uniqueness |
| `FirstName` | text | yes | no | Given name |
| `LastName` | text | yes | no | Family name |
| `PhoneNumber` | text | no | no | Telephone number |
| `SystemRole` | enum | yes | no | System-level role |
| `Status` | enum | yes | no | User-account status |
| `EmailConfirmed` | boolean | yes | no | Whether the email address has been confirmed |
| `LastLoginAt` | timestamp | no | no | Last successful login |
| `CreatedAt` | timestamp | yes | no | Creation timestamp |
| `CreatedByUserId` | UUID / GUID | no | no | User who created the record |
| `UpdatedAt` | timestamp | no | no | Last update timestamp |
| `UpdatedByUserId` | UUID / GUID | no | no | User who last updated the record |
| `DeactivatedAt` | timestamp | no | no | Deactivation timestamp |
| `DeactivatedByUserId` | UUID / GUID | no | no | User who deactivated the account |
| `RowVersion` | version value | yes | no | Optimistic concurrency control |

## 3.3. SystemRole

| Value | Meaning |
|---|---|
| `None` | No elevated system-wide permissions |
| `Admin` | Full system-wide access |

## 3.4. UserStatus

| Value | Meaning |
|---|---|
| `Invited` | Invited but not yet activated |
| `Active` | Active and allowed to sign in |
| `Suspended` | Temporarily suspended |
| `Deactivated` | Permanently or indefinitely deactivated |

## 3.5. Relationships

| Related entity | Cardinality | Description |
|---|---:|---|
| `CustomerMembership` | 1:N | A user may belong to multiple customer companies |
| `Maintenance` | 1:N | Assigned or performing technician |
| `ServiceReport` | 1:N | Primary technician, creator, or updater |
| `Issue` | 1:N | Reporter, assignee, resolver, or closer |
| `WorkSheet` | 1:N | Issuer |
| `Document` | 1:N | Uploader or deleter |
| `Notification` | 1:N | Recipient or canceller |
| `User` | self-referencing 1:N | Audit relationships |

## 3.6. Constraints

- The normalized email address must be unique system-wide.
- Only users with `Active` status may sign in.
- An `Admin` can access every customer company system-wide.
- A non-admin user can access a customer company only through an active membership.
- Deactivating a user must not remove historical records.

---

# 4. CustomerCompany

## 4.1. Business Meaning

The `CustomerCompany` represents a customer company, organization, or institution managed in the system.

## 4.2. Fields

| Field | Type | Required | Unique | Description |
|---|---|---:|---:|---|
| `Id` | UUID / GUID | yes | yes | Primary key |
| `Name` | text | yes | no | Short or commonly used company name |
| `LegalName` | text | no | no | Full legal name |
| `CompanyRegistrationNumber` | text | no | recommended | Company registration number |
| `TaxNumber` | text | no | recommended | Tax number |
| `Status` | enum | yes | no | Customer-company status |
| `BillingEmail` | text | no | no | Billing email address |
| `PhoneNumber` | text | no | no | Main telephone number |
| `WebsiteUrl` | text | no | no | Website |
| `ContactName` | text | no | no | Primary contact person |
| `ContactEmail` | text | no | no | Contact email address |
| `ContactPhoneNumber` | text | no | no | Contact telephone number |
| `BillingCountryCode` | text | no | no | Billing-address country code |
| `BillingPostalCode` | text | no | no | Postal code |
| `BillingCity` | text | no | no | City |
| `BillingAddressLine` | text | no | no | Street, building number, and additional address data |
| `Notes` | long text | no | no | Internal notes |
| `CreatedAt` | timestamp | yes | no | Creation timestamp |
| `CreatedByUserId` | UUID / GUID | no | no | Creator |
| `UpdatedAt` | timestamp | no | no | Last update timestamp |
| `UpdatedByUserId` | UUID / GUID | no | no | Last updater |
| `ArchivedAt` | timestamp | no | no | Archiving timestamp |
| `ArchivedByUserId` | UUID / GUID | no | no | User who archived the company |
| `RowVersion` | version value | yes | no | Concurrency control |

## 4.3. CustomerCompanyStatus

- `Active`
- `Suspended`
- `Archived`

## 4.4. Relationships

| Related entity | Cardinality |
|---|---:|
| `CustomerMembership` | 1:N |
| `Site` | 1:N |
| `Notification` | 1:N |

## 4.5. Constraints

- The company name is required.
- The tax number, when supplied, must be unique.
- The company registration number, when supplied, must be unique.
- No new site or membership may be created for an archived company.
- Archiving must be used instead of physical deletion.

---

# 5. CustomerMembership

## 5.1. Business Meaning

The `CustomerMembership` represents the relationship between a `User` and a `CustomerCompany`.

The record determines which customer company the user can access, with which role, and in which membership state.

## 5.2. Fields

| Field | Type | Required | Unique | Description |
|---|---|---:|---:|---|
| `Id` | UUID / GUID | yes | yes | Primary key |
| `UserId` | UUID / GUID | yes | composite | User |
| `CustomerCompanyId` | UUID / GUID | yes | composite | Customer company |
| `Role` | enum | yes | no | Company-specific role |
| `Status` | enum | yes | no | Membership status |
| `CreatedAt` | timestamp | yes | no | Creation timestamp |
| `CreatedByUserId` | UUID / GUID | no | no | Creator |
| `ActivatedAt` | timestamp | no | no | Activation timestamp |
| `ActivatedByUserId` | UUID / GUID | no | no | User who activated the membership |
| `SuspendedAt` | timestamp | no | no | Suspension timestamp |
| `SuspendedByUserId` | UUID / GUID | no | no | User who suspended the membership |
| `DeactivatedAt` | timestamp | no | no | Deactivation timestamp |
| `DeactivatedByUserId` | UUID / GUID | no | no | User who deactivated the membership |
| `Note` | text | no | no | Internal note |
| `RowVersion` | version value | yes | no | Concurrency control |

## 5.3. CustomerMembershipRole

| Value | Permissions |
|---|---|
| `Customer` | View data, report issues, download documents |
| `MaintenanceTechnician` | Manage equipment, maintenance, issues, and reports |
| `CompanyAdministrator` | Optional future role for administering the specific customer company |

## 5.4. CustomerMembershipStatus

- `Invited`
- `Active`
- `Suspended`
- `Deactivated`

## 5.5. Relationships

| Related entity | Cardinality |
|---|---:|
| `User` | N:1 |
| `CustomerCompany` | N:1 |
| auditing `User` records | N:1 optional |

## 5.6. Constraints

- `UserId + CustomerCompanyId` must be unique.
- Only an `Active` membership grants access.
- The role applies only to the associated customer company.
- When a deactivated membership is reactivated, the existing record should be updated rather than duplicated.
- An admin does not require a membership for access.

---

# 6. Site

## 6.1. Business Meaning

The `Site` represents a specific physical location belonging to a customer company.

## 6.2. Fields

| Field | Type | Required | Unique | Description |
|---|---|---:|---:|---|
| `Id` | UUID / GUID | yes | yes | Primary key |
| `CustomerCompanyId` | UUID / GUID | yes | no | Owning customer company |
| `Name` | text | yes | recommended within company | Site name |
| `Code` | short text | no | recommended within company | Internal site code |
| `Status` | enum | yes | no | Site status |
| `CountryCode` | short text | yes | no | Country code |
| `PostalCode` | short text | yes | no | Postal code |
| `City` | text | yes | no | City |
| `AddressLine` | text | yes | no | Street, building number, building details |
| `Latitude` | decimal | no | no | Latitude |
| `Longitude` | decimal | no | no | Longitude |
| `ContactName` | text | no | no | Site contact person |
| `ContactEmail` | text | no | no | Contact email address |
| `ContactPhoneNumber` | text | no | no | Contact telephone number |
| `Notes` | long text | no | no | Internal notes |
| audit and archive fields | mixed | yes/partial | no | Creation, update, and archiving |
| `RowVersion` | version value | yes | no | Concurrency control |

## 6.3. SiteStatus

- `Active`
- `Inactive`
- `Archived`

## 6.4. Relationships

| Related entity | Cardinality |
|---|---:|
| `CustomerCompany` | N:1 |
| `Equipment` | 1:N |
| `ServiceReport` | 1:N |

## 6.5. Constraints

- Every site belongs to exactly one customer company.
- `CustomerCompanyId + Code`, when a code is provided, must be unique.
- No new equipment or ServiceReport may be created for an archived site.
- A site with equipment must not be physically deleted.

---

# 7. EquipmentType

## 7.1. Business Meaning

The `EquipmentType` represents an equipment category and its default inspection rules.

## 7.2. Fields

| Field | Type | Required | Unique | Description |
|---|---|---:|---:|---|
| `Id` | UUID / GUID | yes | yes | Primary key |
| `Name` | text | yes | yes | Type name |
| `Code` | short text | yes | yes | Stable type identifier |
| `Description` | long text | no | no | Type description |
| `DefaultInspectionIntervalMonths` | integer | no | no | Default inspection interval |
| `RequiresSerialNumber` | boolean | yes | no | Whether a serial number is required |
| `RequiresManufacturingYear` | boolean | yes | no | Whether manufacturing year is required |
| `RequiresCommissioningDate` | boolean | yes | no | Whether commissioning date is required |
| `IsActive` | boolean | yes | no | Whether it can be selected for new equipment |
| audit fields | mixed | yes/partial | no | Creation and updates |
| `RowVersion` | version value | yes | no | Concurrency control |

## 7.3. Relationships

| Related entity | Cardinality |
|---|---:|
| `Equipment` | 1:N |

## 7.4. Constraints

- The name and code must each be unique system-wide.
- The inspection interval must be a positive integer.
- A type already in use must not be deleted.
- An inactive type must not be assigned to new equipment.

---

# 8. Equipment

## 8.1. Business Meaning

The `Equipment` represents a specific physical fire-safety or safety device.

## 8.2. Fields

| Field | Type | Required | Unique | Description |
|---|---|---:|---:|---|
| `Id` | UUID / GUID | yes | yes | Internal primary key |
| `SiteId` | UUID / GUID | yes | no | Site |
| `EquipmentTypeId` | UUID / GUID | yes | no | Equipment type |
| `AssetIdentifier` | short text | yes | yes | Human-readable equipment identifier |
| `PublicToken` | UUID or secure token | yes | yes | Public QR-code identifier |
| `SerialNumber` | text | conditional | no | Manufacturer serial number |
| `Manufacturer` | text | no | no | Manufacturer |
| `Model` | text | no | no | Model |
| `ManufacturingYear` | year | conditional | no | Manufacturing year |
| `CommissionedOn` | date | conditional | no | Commissioning date |
| `Building` | text | no | no | Building |
| `Floor` | text | no | no | Floor or level |
| `Room` | text | no | no | Room |
| `ExactLocation` | text | no | no | Precise location description |
| `InspectionIntervalMonths` | integer | no | no | Equipment-specific inspection interval |
| `LastInspectionDate` | date | no | no | Last inspection date |
| `NextInspectionDueDate` | date | no | no | Next inspection due date |
| `LifecycleStatus` | enum | yes | no | Stored lifecycle status |
| `OutOfServiceReason` | long text | no | no | Reason for being out of service |
| `DecommissionedOn` | date | no | no | Final decommissioning date |
| `Notes` | long text | no | no | Notes |
| audit and archive fields | mixed | yes/partial | no | Creation, update, archiving |
| `RowVersion` | version value | yes | no | Concurrency control |

## 8.3. EquipmentLifecycleStatus

- `Active`
- `OutOfService`
- `Decommissioned`
- `Archived`

## 8.4. Calculated Operational Status

The following are calculated business states rather than necessarily stored fields:

| Status | Condition |
|---|---|
| `Ok` | Active, no significant open issue, due date is not approaching |
| `Warning` | Inspection due within 30 days or a non-critical issue is open |
| `Expired` | Next inspection due date has passed |
| `Faulty` | A high-severity or critical active issue exists |
| `OutOfService` | Derived from stored lifecycle status |
| `Decommissioned` | Permanently withdrawn from service |

## 8.5. Relationships

| Related entity | Cardinality |
|---|---:|
| `Site` | N:1 |
| `EquipmentType` | N:1 |
| `Maintenance` | 1:N |
| `Issue` | 1:N |
| `Document` | 1:N |
| `Notification` | 1:N |

## 8.6. Constraints

- `AssetIdentifier` must be unique system-wide.
- `PublicToken` must be unique system-wide.
- The manufacturing year must not be in the future.
- The next inspection date must not precede the last inspection date.
- New normal maintenance or issue records must not be created for decommissioned equipment.
- Site transfer must be implemented as a separately audited business operation.

---

# 9. ServiceReport

## 9.1. Business Meaning

The `ServiceReport` groups together a complete maintenance or service visit at a particular site.

A ServiceReport may contain multiple `Maintenance` records for multiple pieces of equipment.

Despite its name, the entity manages the complete lifecycle:

- planned;
- in progress;
- completed;
- cancelled.

## 9.2. Fields

| Field | Type | Required | Unique | Description |
|---|---|---:|---:|---|
| `Id` | UUID / GUID | yes | yes | Primary key |
| `ReportNumber` | short text | yes | yes | Report or service-operation identifier |
| `SiteId` | UUID / GUID | yes | no | Affected site |
| `Status` | enum | yes | no | Workflow status |
| `ScheduledStartAt` | timestamp | no | no | Planned start |
| `ScheduledEndAt` | timestamp | no | no | Planned completion |
| `StartedAt` | timestamp | no | no | Actual start |
| `CompletedAt` | timestamp | no | no | Actual completion |
| `PrimaryTechnicianUserId` | UUID / GUID | no | no | Primary technician |
| `Description` | long text | no | no | Purpose of the work |
| `CustomerRepresentativeName` | text | no | no | Customer’s on-site representative |
| `CustomerRepresentativeTitle` | text | no | no | Position or role |
| `GeneralFindings` | long text | no | no | General findings |
| `Notes` | long text | no | no | Internal notes |
| `CancellationReason` | long text | required on cancellation | no | Reason for cancellation |
| audit and cancellation fields | mixed | yes/partial | no | Creation, update, cancellation |
| `RowVersion` | version value | yes | no | Concurrency control |

## 9.3. ServiceReportStatus

- `Planned`
- `InProgress`
- `Completed`
- `Cancelled`

## 9.4. Relationships

| Related entity | Cardinality |
|---|---:|
| `Site` | N:1 |
| `Maintenance` | 1:N |
| `WorkSheet` | 1:N |
| `Document` | 1:N |
| `User` as primary technician | N:1 optional |

## 9.5. Constraints

- A ServiceReport belongs to exactly one site.
- The equipment referenced by all related Maintenance records must belong to the same site.
- A ServiceReport may be marked `Completed` only when all required Maintenance items are completed.
- A cancellation reason is required for `Cancelled` status.
- A completed record must not be freely editable by ordinary users.

---

# 10. Maintenance

## 10.1. Business Meaning

The `Maintenance` represents an inspection, repair, maintenance task, or other technical operation performed on one specific piece of equipment.

## 10.2. Fields

| Field | Type | Required | Unique | Description |
|---|---|---:|---:|---|
| `Id` | UUID / GUID | yes | yes | Primary key |
| `ServiceReportId` | UUID / GUID | yes | no | Parent ServiceReport |
| `EquipmentId` | UUID / GUID | yes | no | Affected equipment |
| `Type` | enum | yes | no | Maintenance type |
| `Status` | enum | yes | no | Workflow status |
| `ScheduledDate` | date | no | no | Planned date |
| `StartedAt` | timestamp | no | no | Start time |
| `PerformedDate` | date | required on completion | no | Actual date performed |
| `AssignedTechnicianUserId` | UUID / GUID | no | no | Assigned technician |
| `PerformedByUserId` | UUID / GUID | required on completion | no | Performing technician |
| `Result` | enum | required on completion | no | Result |
| `WorkDescription` | long text | no | no | Work performed |
| `Findings` | long text | no | no | Findings |
| `Recommendation` | long text | no | no | Recommended follow-up |
| `NextInspectionDueDate` | date | no | no | Next due date |
| `CancellationReason` | long text | required on cancellation | no | Reason for interruption or cancellation |
| `CompletedAt` | timestamp | required on completion | no | Completion timestamp |
| `CompletedByUserId` | UUID / GUID | required on completion | no | User who completed the record |
| audit fields | mixed | yes/partial | no | Creation and updates |
| `RowVersion` | version value | yes | no | Concurrency control |

## 10.3. MaintenanceType

- `Inspection`
- `PreventiveMaintenance`
- `Repair`
- `Replacement`
- `Commissioning`
- `Decommissioning`

## 10.4. MaintenanceStatus

- `Planned`
- `InProgress`
- `Completed`
- `Cancelled`

## 10.5. MaintenanceResult

- `Passed`
- `PassedWithRemarks`
- `Failed`
- `RepairRequired`
- `ReplacementRequired`

## 10.6. Relationships

| Related entity | Cardinality |
|---|---:|
| `ServiceReport` | N:1 |
| `Equipment` | N:1 |
| `User` as assigned technician | N:1 optional |
| `User` as performing technician | N:1 optional |
| `Document` | 1:N |
| `Issue` | N:M through `MaintenanceIssue` |

## 10.7. Constraints

- Only a technician with a suitable membership for the equipment’s customer company may be assigned.
- `Result`, performing technician, and actual date are required for `Completed` status.
- A failed result may be linked to an issue or create a new issue.
- On completion, the equipment’s inspection dates may be updated in the same transaction.
- A completed record must not be freely edited.

---

# 11. MaintenanceIssue

## 11.1. Business Meaning

The `MaintenanceIssue` junction table manages the N:M relationship between Maintenance and Issue.

## 11.2. Fields

| Field | Type | Required | Unique | Description |
|---|---|---:|---:|---|
| `MaintenanceId` | UUID / GUID | yes | composite | Maintenance record |
| `IssueId` | UUID / GUID | yes | composite | Issue |
| `RelationType` | enum | yes | no | Meaning of the relationship |
| `CreatedAt` | timestamp | yes | no | Relationship creation timestamp |
| `CreatedByUserId` | UUID / GUID | no | no | Creator |

## 11.3. RelationType

- `Detected`
- `Resolved`
- `Related`

## 11.4. Constraints

- `MaintenanceId + IssueId + RelationType` may be unique.
- Both records must refer to the same equipment.
- Creating the relationship requires authorization.

---

# 12. Issue

## 12.1. Business Meaning

The `Issue` represents a defect, damage, or operational anomaly associated with a piece of equipment.

## 12.2. Fields

| Field | Type | Required | Unique | Description |
|---|---|---:|---:|---|
| `Id` | UUID / GUID | yes | yes | Primary key |
| `EquipmentId` | UUID / GUID | yes | no | Affected equipment |
| `IssueNumber` | short text | yes | yes | Human-readable issue identifier |
| `Title` | text | yes | no | Short summary |
| `Description` | long text | yes | no | Detailed description |
| `Severity` | enum | yes | no | Severity |
| `Status` | enum | yes | no | Workflow status |
| `ReportedAt` | timestamp | yes | no | Reporting timestamp |
| `ReportedByUserId` | UUID / GUID | yes | no | Reporter |
| `AssignedToUserId` | UUID / GUID | no | no | Assignee |
| `AcknowledgedAt` | timestamp | no | no | Acknowledgement timestamp |
| `AcknowledgedByUserId` | UUID / GUID | no | no | User who acknowledged the issue |
| `Resolution` | long text | required on resolution | no | Resolution description |
| `ResolvedAt` | timestamp | no | no | Resolution timestamp |
| `ResolvedByUserId` | UUID / GUID | no | no | Resolver |
| `ClosedAt` | timestamp | no | no | Final closure timestamp |
| `ClosedByUserId` | UUID / GUID | no | no | User who closed the issue |
| `RejectionReason` | long text | required on rejection | no | Rejection reason |
| audit fields | mixed | yes/partial | no | Creation and updates |
| `RowVersion` | version value | yes | no | Concurrency control |

## 12.3. IssueSeverity

- `Low`
- `Medium`
- `High`
- `Critical`

## 12.4. IssueStatus

- `Open`
- `Acknowledged`
- `InProgress`
- `Resolved`
- `Closed`
- `Rejected`

## 12.5. Relationships

| Related entity | Cardinality |
|---|---:|
| `Equipment` | N:1 |
| `User` as reporter | N:1 |
| `User` as assignee | N:1 optional |
| `Document` | 1:N |
| `Maintenance` | N:M |
| `Notification` | 1:N |

## 12.6. Constraints

- Title and description are required.
- The reporter must have access to the equipment’s customer company.
- A resolution description is required for `Resolved` status.
- A rejection reason is required for `Rejected` status.
- `Closed` status may only be reached from `Resolved`.
- A high-severity or critical active issue changes the equipment’s calculated status to `Faulty`.

---

# 13. WorkSheet

## 13.1. Business Meaning

The `WorkSheet` is the official certificate issued by the maintenance company to the customer, confirming that the work was officially completed.

The WorkSheet is not the same as:

- the ServiceReport workflow record;
- the Maintenance technical item;
- the PDF file.

The WorkSheet is the business and legal representation of the certificate. Its final PDF is stored through the `Document` subsystem.

## 13.2. Fields

| Field | Type | Required | Unique | Description |
|---|---|---:|---:|---|
| `Id` | UUID / GUID | yes | yes | Primary key |
| `WorkSheetNumber` | short text | yes | yes | Official worksheet number |
| `ServiceReportId` | UUID / GUID | yes | no | Underlying service operation |
| `Status` | enum | yes | no | Issuance status |
| `IssuedAt` | timestamp | required when issued | no | Issuance timestamp |
| `IssuedByUserId` | UUID / GUID | required when issued | no | Issuing user |
| `IssuerCompanyName` | text | yes | no | Maintenance-company name at issuance |
| `IssuerCompanyTaxNumber` | text | no | no | Maintenance-company tax number |
| `IssuerCompanyAddress` | text | no | no | Maintenance-company address |
| `CustomerCompanyName` | text | yes | no | Customer name at issuance |
| `CustomerCompanyTaxNumber` | text | no | no | Customer tax number |
| `CustomerCompanyAddress` | text | no | no | Customer address |
| `SiteName` | text | yes | no | Site name at issuance |
| `SiteAddress` | text | yes | no | Site address at issuance |
| `WorkStartedAt` | timestamp | no | no | Actual work start |
| `WorkCompletedAt` | timestamp | yes | no | Actual work completion |
| `WorkSummary` | long text | yes | no | Summary of work performed |
| `GeneralFindings` | long text | no | no | General findings |
| `CustomerRepresentativeName` | text | no | no | Recipient or customer representative |
| `CustomerRepresentativeTitle` | text | no | no | Position or title |
| `AcceptedAt` | timestamp | no | no | Receipt or acceptance timestamp |
| `VersionNumber` | integer | yes | composite | Worksheet version |
| `ReplacesWorkSheetId` | UUID / GUID | no | no | Previous worksheet replaced by this version |
| `GeneratedDocumentId` | UUID / GUID | required after issuance | yes | Final PDF document |
| `CreatedAt` | timestamp | yes | no | Record creation timestamp |
| `CreatedByUserId` | UUID / GUID | yes | no | Creator |
| `RowVersion` | version value | yes | no | Concurrency control |

## 13.3. WorkSheetStatus

- `Draft`
- `Issued`
- `Accepted`
- `Superseded`
- `Cancelled`

## 13.4. Relationships

| Related entity | Cardinality |
|---|---:|
| `ServiceReport` | N:1 |
| `Document` | 1:N |
| `User` as issuer | N:1 |
| `WorkSheet` self-reference | N:1 optional |

## 13.5. Constraints

- A WorkSheet may only be issued for a `Completed` ServiceReport.
- The worksheet number is unique and immutable after issuance.
- An issued worksheet must not be modified through normal editing.
- Corrections require a new version or corrective WorkSheet.
- Previous versions must not be overwritten.
- Company and site details at the time of issuance must be preserved as a snapshot.
- Multiple WorkSheets may belong to a ServiceReport for versioning purposes.
- Only one current valid issued version may exist at a time.

---

# 14. Document

## 14.1. Business Meaning

The `Document` represents file metadata.

The actual binary file is stored in an object store or file system. The database stores only metadata and business relationships.

## 14.2. Fields

| Field | Type | Required | Unique | Description |
|---|---|---:|---:|---|
| `Id` | UUID / GUID | yes | yes | Primary key |
| `Type` | enum | yes | no | Document type |
| `OriginalFileName` | text | yes | no | Original filename |
| `StorageKey` | text | yes | yes | Unique storage key |
| `ContentType` | text | yes | no | MIME type |
| `FileExtension` | short text | yes | no | File extension |
| `SizeInBytes` | integer | yes | no | File size |
| `Sha256Hash` | text | no | no | Integrity hash |
| `Title` | text | no | no | Display title |
| `Description` | long text | no | no | Description |
| `EquipmentId` | UUID / GUID | conditional | no | Equipment relationship |
| `ServiceReportId` | UUID / GUID | conditional | no | ServiceReport relationship |
| `MaintenanceId` | UUID / GUID | conditional | no | Maintenance relationship |
| `IssueId` | UUID / GUID | conditional | no | Issue relationship |
| `WorkSheetId` | UUID / GUID | conditional | no | WorkSheet relationship |
| `GeneratedBySystem` | boolean | yes | no | Whether generated by the system |
| `DocumentVersion` | integer | no | no | Document version |
| `UploadedAt` | timestamp | yes | no | Upload or generation timestamp |
| `UploadedByUserId` | UUID / GUID | no | no | Uploader |
| `DeletedAt` | timestamp | no | no | Soft-delete timestamp |
| `DeletedByUserId` | UUID / GUID | no | no | User who deleted the document |
| `RowVersion` | version value | yes | no | Concurrency control |

## 14.3. DocumentType

- `EquipmentPhoto`
- `ServiceReportPhoto`
- `MaintenancePhoto`
- `IssuePhoto`
- `WorkSheetPdf`
- `WorkSheetAttachment`
- `CustomerSignature`
- `InspectionReport`
- `Certificate`
- `ManufacturerDocument`
- `Other`

## 14.4. Relationships

A document must have exactly one direct business parent:

- `Equipment`
- `ServiceReport`
- `Maintenance`
- `Issue`
- `WorkSheet`

## 14.5. Constraints

- Exactly one business-parent relationship is required.
- `StorageKey` must be unique.
- File size must be positive and below the configured limit.
- Only allowed MIME types may be uploaded.
- The original filename must not be used as the storage key.
- Authorization must always be checked before download.
- The PDF of an issued WorkSheet must not be physically deleted through normal operations.

---

# 15. Notification

## 15.1. Business Meaning

The `Notification` represents a specific system notification that is pending or has already been sent.

The record also serves as a delivery log and retry unit.

## 15.2. Fields

| Field | Type | Required | Unique | Description |
|---|---|---:|---:|---|
| `Id` | UUID / GUID | yes | yes | Primary key |
| `Type` | enum | yes | no | Notification type |
| `Channel` | enum | yes | no | Delivery channel |
| `Status` | enum | yes | no | Delivery status |
| `RecipientUserId` | UUID / GUID | no | no | Internal recipient |
| `RecipientEmail` | text | yes | no | Email used for delivery |
| `CustomerCompanyId` | UUID / GUID | yes | no | Customer-company scope |
| `EquipmentId` | UUID / GUID | no | no | Related equipment |
| `ServiceReportId` | UUID / GUID | no | no | Related ServiceReport |
| `MaintenanceId` | UUID / GUID | no | no | Related Maintenance |
| `IssueId` | UUID / GUID | no | no | Related Issue |
| `WorkSheetId` | UUID / GUID | no | no | Related WorkSheet |
| `Subject` | text | yes | no | Subject |
| `Body` | long text | yes | no | Body |
| `ScheduledAt` | timestamp | yes | no | Planned delivery time |
| `SentAt` | timestamp | no | no | Successful delivery time |
| `FailedAt` | timestamp | no | no | Failure time |
| `AttemptCount` | integer | yes | no | Number of attempts |
| `NextAttemptAt` | timestamp | no | no | Next retry time |
| `LastError` | long text | no | no | Most recent error |
| `ExternalMessageId` | text | no | no | External provider identifier |
| `DeduplicationKey` | text | yes | yes | Duplicate-prevention key |
| `CreatedAt` | timestamp | yes | no | Record creation |
| `CancelledAt` | timestamp | no | no | Cancellation timestamp |
| `CancelledByUserId` | UUID / GUID | no | no | User who cancelled the notification |
| `RowVersion` | version value | yes | no | Concurrency control |

## 15.3. NotificationType

- `InspectionDueIn30Days`
- `InspectionDueIn14Days`
- `InspectionDueIn7Days`
- `InspectionDueToday`
- `InspectionExpired`
- `IssueReported`
- `IssueAssigned`
- `IssueResolved`
- `MaintenanceAssigned`
- `MaintenanceCompleted`
- `ServiceReportCompleted`
- `WorkSheetIssued`
- `UserInvitation`
- `Custom`

## 15.4. NotificationChannel

- `Email`
- `InApp`
- `Sms`

Only `Email` is required for the first version.

## 15.5. NotificationStatus

- `Pending`
- `Processing`
- `Sent`
- `Failed`
- `Cancelled`
- `DeadLettered`

## 15.6. Relationships

| Related entity | Cardinality |
|---|---:|
| `User` as recipient | N:1 optional |
| `CustomerCompany` | N:1 |
| `Equipment` | N:1 optional |
| `ServiceReport` | N:1 optional |
| `Maintenance` | N:1 optional |
| `Issue` | N:1 optional |
| `WorkSheet` | N:1 optional |

## 15.7. Constraints

- `RecipientEmail` is required.
- `DeduplicationKey` must be unique.
- `SentAt` may only be populated for `Sent` status.
- `LastError` must be recorded after a failed attempt.
- The attempt count must not be negative.
- The delivered subject and body must be retained unchanged as a log.
- Notification records must not be physically deleted during normal operation.

---

# 16. Consolidated Relational Model

```text
User
 ├── CustomerMembership ── CustomerCompany
 ├── Maintenance
 ├── Issue
 ├── ServiceReport
 ├── WorkSheet
 ├── Document
 └── Notification

CustomerCompany
 └── Site
      ├── Equipment
      │    ├── Maintenance
      │    │    ├── Document
      │    │    └── MaintenanceIssue ── Issue
      │    ├── Issue
      │    │    └── Document
      │    ├── Document
      │    └── Notification
      │
      └── ServiceReport
           ├── Maintenance
           ├── Document
           └── WorkSheet
                ├── Document
                └── WorkSheet version chain

EquipmentType
 └── Equipment
```

---

# 17. Cardinality Summary

| Source | Relationship | Target |
|---|---:|---|
| `User` | 1:N | `CustomerMembership` |
| `CustomerCompany` | 1:N | `CustomerMembership` |
| `CustomerCompany` | 1:N | `Site` |
| `Site` | 1:N | `Equipment` |
| `EquipmentType` | 1:N | `Equipment` |
| `Site` | 1:N | `ServiceReport` |
| `ServiceReport` | 1:N | `Maintenance` |
| `Equipment` | 1:N | `Maintenance` |
| `Equipment` | 1:N | `Issue` |
| `Maintenance` | N:M | `Issue` |
| `ServiceReport` | 1:N | `WorkSheet` |
| `WorkSheet` | 1:N | `Document` |
| `Equipment` | 1:N | `Document` |
| `ServiceReport` | 1:N | `Document` |
| `Maintenance` | 1:N | `Document` |
| `Issue` | 1:N | `Document` |
| `CustomerCompany` | 1:N | `Notification` |

---

# 18. Principal Business Invariants

1. A `User` may belong to multiple customer companies.
2. A user may have a different role at each customer company.
3. The customer-company-specific role is stored in `CustomerMembership`.
4. System-level administrator access is stored in `User.SystemRole`.
5. A site belongs to exactly one customer company.
6. Equipment belongs to exactly one site and one equipment type.
7. A ServiceReport belongs to exactly one site.
8. A ServiceReport may group multiple Maintenance items.
9. The equipment referenced by a ServiceReport’s Maintenance records must belong to the same site.
10. A WorkSheet may only be issued for a completed ServiceReport.
11. The WorkSheet is the official proof-of-performance business record.
12. The WorkSheet PDF is stored through the Document subsystem.
13. Historical data of an issued worksheet must not be changed retroactively.
14. Document files are stored outside the database in a file or object store.
15. Customer-company-level authorization must always be checked before document download.
16. Equipment operational status is partly calculated.
17. Duplicate notifications are prevented by a unique deduplication key.
18. Historical and official records must not be physically deleted.

---

# 19. Recommended Next Design Steps

After approving this document, the recommended sequence is:

1. finalize required and optional fields;
2. finalize enum values;
3. define database naming conventions;
4. define delete behavior and foreign-key actions;
5. create the concrete database schema and migration plan;
6. detail domain business rules and state transitions;
7. design API resources and endpoints;
8. map frontend screens and forms to the entities.
