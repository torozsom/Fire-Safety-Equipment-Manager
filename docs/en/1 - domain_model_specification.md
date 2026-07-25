# Fire Safety Equipment Management System
## Domain Model Developer Specification

**Document status:** approved domain model  
**Language:** English  
**Target audience:** backend and frontend developers, system architects, testers  
**Technology context:** ASP.NET Core Web API backend, Angular frontend

---

## 1. Purpose of the Document

This document describes the approved high-level domain model of the Fire Safety Equipment Management System.

The document is limited to defining:

- the main domain entities of the system;
- the relationships between the entities;
- the assignment model between users and customer companies;
- the operation of global roles;
- the principles governing access to customer data;
- domain-level cardinalities and integrity rules;
- high-level requirements for authorization checks.

The detailed fields, validation rules, state machines, API endpoints, and database migrations of the individual entities are outside the scope of this document. They will be defined in a separate design phase.

---

## 2. Domain Model Overview

The central business objects of the system are:

1. `ApplicationUser` – a user of the system;
2. `Customer` – a customer company;
3. `CustomerMembership` – an assignment between a user and a customer company;
4. `Site` – a site belonging to a customer company;
5. `Equipment` – a fire safety or other safety device registered at a site;
6. `Maintenance` – a maintenance event associated with a piece of equipment;
7. `Issue` – a fault report or issue event associated with a piece of equipment;
8. `Document` – a document, photograph, or other file associated with a piece of equipment.

The central hierarchy of the model is:

```text
ApplicationUser
    └── CustomerMembership
            └── Customer
                    └── Site
                            └── Equipment
                                    ├── Maintenance
                                    ├── Issue
                                    └── Document
```

The `CustomerMembership` junction entity allows one user to access multiple customer companies and allows multiple users to belong to one customer company.

---

## 3. Main Domain Entities

### 3.1. ApplicationUser

`ApplicationUser` represents an authenticated natural person in the system.

Responsibilities:

- identifying the user;
- storing the user's global role;
- serving as the connection point for assignments to customer companies;
- representing the actor who performs auditable operations.

A user must have exactly one global role.

The role does not vary by customer company. For example, if a user is a maintenance technician, the user acts as a maintenance technician for every customer to which the user has an active assignment.

### 3.2. Customer

`Customer` represents a customer company.

Responsibilities:

- grouping the sites belonging to the customer;
- serving as the target of relationships for users who have access;
- defining the logical boundary of customer-level data isolation.

A customer company may have:

- multiple users;
- multiple sites.

Users are not connected to `Customer` through a direct foreign key. They are connected through the `CustomerMembership` junction entity.

### 3.3. CustomerMembership

`CustomerMembership` is the explicit junction entity between `ApplicationUser` and `Customer`.

Meaning of a record:

> The specified user is assigned to the specified customer company and may perform operations on its data according to the user's global role.

Use of the junction entity is mandatory. The implicit many-to-many relationship provided by Entity Framework Core is not recommended because the relationship has independent business meaning and its own lifecycle.

Benefits of the junction entity:

- makes the assignment auditable;
- allows the relationship to be activated and deactivated;
- allows the creation time and creator to be stored;
- can later be extended with temporal or custom access restrictions;
- allows access to be revoked instead of physically deleting the relationship.

The relationship must distinguish at least between active and inactive states. The detailed status model may later be extended with states such as invited, suspended, or removed.

### 3.4. Site

`Site` represents a site belonging to a customer company.

Responsibilities:

- grouping location-bound equipment;
- representing the customer's organizational or geographical structure;
- supporting site-based filtering of equipment lists and reports.

Each site belongs to exactly one customer company.

A site may contain multiple pieces of equipment.

### 3.5. Equipment

`Equipment` represents a specific registered fire safety or other safety device.

Each piece of equipment belongs to exactly one site.

A piece of equipment may have multiple:

- maintenance events;
- issue reports;
- documents or photographs.

Equipment is one of the central aggregation points of the application. The lifecycle of events and files associated with equipment is interpreted in the context of that equipment.

### 3.6. Maintenance

`Maintenance` represents a maintenance or inspection event associated with a piece of equipment.

Each maintenance event belongs to exactly one piece of equipment.

A piece of equipment may have multiple maintenance events over time. Together, these events form the equipment's maintenance history.

The detailed fields, outcomes, work-order relationships, and report relationships of maintenance events will be defined in a separate specification.

### 3.7. Issue

`Issue` represents a fault report or other fault-related event associated with a piece of equipment.

Each issue report belongs to exactly one piece of equipment.

A piece of equipment may have multiple issue reports. These reports may be independent of one another and may have their own state, description, timestamp, and attachments.

The detailed issue states and closure workflow will be defined later.

### 3.8. Document

`Document` represents a file associated with a piece of equipment.

Examples include:

- photographs;
- inspection reports;
- work orders;
- certificates;
- other related documents.

In the current domain model, documents are primarily associated with the `Equipment` entity.

During detailed design, it may be decided whether documents must also be linked directly to `Maintenance` or `Issue` entities. This would refine, rather than change, the current high-level model.

---

## 4. Relationships and Cardinalities

### 4.1. User and Customer Company

```text
ApplicationUser 1 ──── * CustomerMembership
Customer        1 ──── * CustomerMembership
```

Therefore:

```text
ApplicationUser * ──── * Customer
```

The many-to-many relationship is implemented through an explicit junction entity.

Rules:

- a user may be assigned to zero, one, or multiple customer companies;
- a customer company may have zero, one, or multiple assigned users;
- the same user may be assigned to the same customer at most once;
- an inactive assignment does not grant access.

### 4.2. Customer Company and Site

```text
Customer 1 ──── * Site
```

Rules:

- each site belongs to exactly one customer company;
- a customer company may have multiple sites;
- a site cannot exist without a customer company.

### 4.3. Site and Equipment

```text
Site 1 ──── * Equipment
```

Rules:

- each piece of equipment belongs to exactly one site;
- a site may contain multiple pieces of equipment;
- equipment cannot exist without a site.

### 4.4. Equipment and Maintenance

```text
Equipment 1 ──── * Maintenance
```

Rules:

- each maintenance event belongs to exactly one piece of equipment;
- a piece of equipment may have multiple maintenance events.

### 4.5. Equipment and Issue

```text
Equipment 1 ──── * Issue
```

Rules:

- each issue report belongs to exactly one piece of equipment;
- a piece of equipment may have multiple issue reports.

### 4.6. Equipment and Document

```text
Equipment 1 ──── * Document
```

Rules:

- each document must be meaningful within the context of at least one piece of equipment;
- a piece of equipment may have multiple documents or photographs.

---

## 5. Role Model

A user must have exactly one global role in the system.

Approved roles:

```text
Admin
MaintenanceTechnician
CustomerUser
```

Business meanings:

| Technical name | Business meaning |
|---|---|
| `Admin` | system administrator or administrator with full access |
| `MaintenanceTechnician` | maintenance technician |
| `CustomerUser` | customer-side user |

The role is stored on `ApplicationUser`. It is not part of `CustomerMembership`.

Consequences:

- a maintenance technician has maintenance-technician permissions for every assigned customer;
- a customer-side user has customer-user permissions for every assigned customer;
- a user cannot be a maintenance technician for one customer and a customer-side user for another;
- changing a user's role affects every active customer assignment.

---

## 6. Access Model

Access consists of two separate questions:

1. **May the user access the specified customer?**
2. **Which operations may the user perform on that customer's data?**

### 6.1. Access to a Customer

A non-admin user may access a customer when an active `CustomerMembership` record exists between the specified user and customer.

Formal rule:

```text
HasCustomerAccess(userId, customerId) =
    an active CustomerMembership exists
    where UserId = userId and CustomerId = customerId
```

### 6.2. Operational Permissions

The user's global role determines operational permissions.

High-level authorization model:

| Role | Customer assignment required | Nature of access |
|---|---:|---|
| `Admin` | no | full, global access |
| `MaintenanceTechnician` | yes | management of equipment and maintenance data for assigned customers |
| `CustomerUser` | yes | viewing data of assigned customers, reporting issues, and accessing documents |

The detailed operation matrix will be developed in a separate authorization specification.

### 6.3. Admin Access

`Admin` is a global role.

Approved rules:

- an admin may access every customer;
- an admin does not require a `CustomerMembership` record;
- an admin has full permissions over every customer, site, piece of equipment, and related record.

### 6.4. Maintenance Technician Access

A `MaintenanceTechnician`:

- may access only actively assigned customers;
- may view all sites of those customers;
- may manage equipment at all such sites;
- may perform permitted maintenance and work-order operations;
- may be assigned to multiple customers.

### 6.5. Customer-Side User Access

A `CustomerUser`:

- may access only actively assigned customers;
- may view all sites and equipment of those customers;
- may create issue reports;
- may download or view available documents;
- has limited modification permissions, to be defined by the detailed authorization matrix.

---

## 7. Customer-Level Data Isolation

`Customer` is the primary data-isolation boundary of the system.

Every customer-bound object must be traceable unambiguously to exactly one customer:

```text
Site.CustomerId
Equipment → Site → Customer
Maintenance → Equipment → Site → Customer
Issue → Equipment → Site → Customer
Document → Equipment → Site → Customer
```

During authorization checks, it is not sufficient to validate only the `customerId` supplied by the client. The actual ownership chain of the resource must also be verified.

Example:

```text
PUT /api/customers/{customerId}/equipment/{equipmentId}
```

The backend must verify that:

1. the equipment identified by `equipmentId` exists;
2. the equipment's site actually belongs to the customer identified by `customerId`;
3. the user may access that customer;
4. the user's role permits the requested modification.

This prevents unauthorized access by manipulating client-side identifiers.

---

## 8. CustomerMembership Lifecycle

Assignments are created through an administrative operation.

Recommended workflow:

1. the admin selects an existing user or invites a new user;
2. the admin selects one or more customer companies;
3. the system creates one `CustomerMembership` record per customer;
4. when the relationship is active, the user may access the customer;
5. when access is revoked, the relationship becomes inactive;
6. the previous relationship may be retained for audit purposes.

The system must prevent duplicate creation of the same active or logically identical relationship.

---

## 9. Data Integrity Rules

### 9.1. Unique User–Customer Relationship

A composite unique constraint is required on `CustomerMembership`:

```text
UNIQUE(UserId, CustomerId)
```

This guarantees that the same user has only one relationship record with the same customer.

If the system later needs to support historical memberships across multiple periods, this uniqueness rule must be revised. In the current model, the status of a single relationship changes over time.

### 9.2. Required Parent Relationships

The following relationships are mandatory:

- `CustomerMembership.UserId`;
- `CustomerMembership.CustomerId`;
- `Site.CustomerId`;
- `Equipment.SiteId`;
- `Maintenance.EquipmentId`;
- `Issue.EquipmentId`;
- `Document.EquipmentId`.

Orphan records are not permitted.

### 9.3. Delete Behavior

General use of physical cascade deletion is not recommended.

Recommended principles:

- use soft deletion or archiving for business-significant records;
- deactivate user–customer relationships instead of deleting them;
- inspect related historical data before deleting a customer, site, or piece of equipment;
- retain maintenance records, issues, and documents for audit and regulatory reasons.

Detailed deletion and archiving rules will be defined in a separate specification.

---

## 10. Navigation Properties – Recommended Structure

The following example is intended only to illustrate the relationships. Final fields and types will be defined later.

```csharp
public class ApplicationUser
{
    public Guid Id { get; set; }

    public UserRole Role { get; set; }

    public ICollection<CustomerMembership> CustomerMemberships { get; set; }
        = new List<CustomerMembership>();
}

public class Customer
{
    public Guid Id { get; set; }

    public ICollection<CustomerMembership> Memberships { get; set; }
        = new List<CustomerMembership>();

    public ICollection<Site> Sites { get; set; }
        = new List<Site>();
}

public class CustomerMembership
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
}

public class Site
{
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public ICollection<Equipment> Equipment { get; set; }
        = new List<Equipment>();
}

public class Equipment
{
    public Guid Id { get; set; }

    public Guid SiteId { get; set; }
    public Site Site { get; set; } = null!;

    public ICollection<Maintenance> MaintenanceEvents { get; set; }
        = new List<Maintenance>();

    public ICollection<Issue> Issues { get; set; }
        = new List<Issue>();

    public ICollection<Document> Documents { get; set; }
        = new List<Document>();
}
```

---

## 11. Entity Framework Core Relationship Configuration – Guideline

The many-to-many relationship must be configured through an explicit junction entity.

Example:

```csharp
public sealed class CustomerMembershipConfiguration
    : IEntityTypeConfiguration<CustomerMembership>
{
    public void Configure(EntityTypeBuilder<CustomerMembership> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.UserId, x.CustomerId })
            .IsUnique();

        builder.HasOne(x => x.User)
            .WithMany(x => x.CustomerMemberships)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Customer)
            .WithMany(x => x.Memberships)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

Use of `OnDelete(DeleteBehavior.Restrict)` is recommended as a guideline to prevent deletion of a user or customer from automatically removing historical assignments.

---

## 12. Backend Authorization Requirements

Authorization checks must always be performed by the backend.

The frontend is responsible only for:

- hiding functions that are not available to the user;
- displaying accessible customers;
- supporting selection of the active customer;
- providing appropriate user feedback.

Frontend restrictions must not be treated as security controls.

Recommended backend authorization sequence:

```text
1. Is the user authenticated?
2. Does the user have the Admin role?
3. If not an admin, does the user have an active CustomerMembership?
4. Does the resource actually belong to the customer being evaluated?
5. Does the role permit the requested operation?
```

Authorization checks should be organized into a central service or authorization policies rather than duplicated independently in each controller.

Possible abstraction:

```csharp
public interface ICustomerAccessService
{
    Task<bool> CanAccessCustomerAsync(
        Guid userId,
        Guid customerId,
        CancellationToken cancellationToken = default);

    Task EnsureCanAccessCustomerAsync(
        Guid userId,
        Guid customerId,
        CancellationToken cancellationToken = default);
}
```

The detailed implementation is a separate design task.

---

## 13. Frontend Behavior for Multiple Customers

When a user belongs to multiple customers, the Angular application must manage customer context.

Recommended behavior:

- when the user belongs to one customer, the system may select it automatically;
- when the user belongs to multiple customers, a customer selector is displayed;
- the active customer is shown in the application header or navigation area;
- switching customers reloads customer-bound lists and dashboard data;
- the customer identifier may be sent to the API as a URL parameter or route segment.

Example:

```http
GET /api/customers/{customerId}/equipment
```

Selection of the active customer is a client-side convenience feature, not proof of authorization.

---

## 14. Aggregate Boundaries – Current Guideline

The detailed Domain-Driven Design aggregate boundaries are not yet final, but the approved model supports the following initial direction:

### Customer Aggregate Context

```text
Customer
├── CustomerMembership
└── Site
```

### Equipment Aggregate Context

```text
Equipment
├── Maintenance
├── Issue
└── Document
```

`Site` belongs to the customer, while `Equipment` is the natural center of its own events and documents.

Final aggregate boundaries must be confirmed after the business operations, transactional requirements, and consistency rules have been designed in detail.

---

## 15. Explicitly Rejected Alternatives

### 15.1. CustomerId Directly on the User Entity

This is not suitable because one user may belong to multiple customers.

### 15.2. Storing the Role on CustomerMembership

This is not required under the current requirements because a user has exactly one role, identical for every customer.

### 15.3. Implicit Many-to-Many Relationship

This is not recommended because the user–customer relationship has business state, audit data, and future extensibility requirements.

### 15.4. Site-Level User Authorization

This is not required in the current model.

Approved rule:

> When a user has access to a customer, the user has role-appropriate access to all sites of that customer.

Site-level assignments may be introduced as a future extension, but they are not part of the first version.

### 15.5. Different Roles per Customer

This is not supported.

For example, a user cannot be a maintenance technician for one customer and a customer-side user for another.

---

## 16. Open Questions for the Next Design Phase

The relationship structure of the domain model is approved. The detailed fields and rules of the entities must be designed in the next phase.

In particular:

- fields of `ApplicationUser` and integration with ASP.NET Core Identity;
- company and contact data of `Customer`;
- statuses and audit fields of `CustomerMembership`;
- address and contact details of `Site`;
- type, identifier, QR code, and status model of `Equipment`;
- workflow and outcomes of `Maintenance`;
- states and closure process of `Issue`;
- file types, storage, and relationship options of `Document`;
- soft deletion and archiving;
- standardization of audit fields;
- handling time zones and date/time types;
- detailed authorization matrix.

---

## 17. Summary of Approved Decisions

1. One `Customer` may have multiple `ApplicationUser` and multiple `Site` records.
2. One `ApplicationUser` may be assigned to multiple `Customer` entities.
3. The user–customer relationship is implemented through the explicit `CustomerMembership` junction entity.
4. A user must have exactly one global role.
5. The role is stored on the `ApplicationUser` entity.
6. The role is identical for every assigned customer.
7. `Admin` has global access to every customer without an assignment.
8. `MaintenanceTechnician` may access only assigned customers but may perform modifications there.
9. `CustomerUser` may access only assigned customers and has limited operational permissions.
10. Access to a customer extends to all sites of that customer.
11. One `Site` may contain multiple `Equipment` entities.
12. One `Equipment` may have multiple `Maintenance`, `Issue`, and `Document` records.
13. A unique `(UserId, CustomerId)` constraint is required on `CustomerMembership`.
14. The backend must verify permissions for every API request.
15. Frontend authorization-based hiding is only a user-interface feature, not a security control.

---

## 18. Final Domain Model Diagram

```text
┌─────────────────────┐
│   ApplicationUser   │
│                     │
│   exactly 1 Role    │
└──────────┬──────────┘
           │ 1
           │
           │ *
┌──────────▼──────────┐
│ CustomerMembership  │
│                     │
│ UserId + CustomerId │
│ active / inactive   │
└──────────┬──────────┘
           │ *
           │
           │ 1
┌──────────▼──────────┐
│      Customer       │
└──────────┬──────────┘
           │ 1
           │
           │ *
┌──────────▼──────────┐
│        Site         │
└──────────┬──────────┘
           │ 1
           │
           │ *
┌──────────▼──────────┐
│      Equipment      │
└───────┬───────┬─────┘
        │       │
        │       │
        ▼       ▼
 Maintenance  Issue
        │
        └──────────────► Document / Photo
```

The diagram is a logical summary of the relationships. The current primary relationship of `Document` is with `Equipment`; the possibility of direct relationships to maintenance and issue reports will be refined in the next design phase.

---

**End of document**
