# EventHub

A full-featured event management and community platform built with ASP.NET Core MVC as a SoftUni ASP.NET Advanced course project. Users can discover and join events, organizers manage their own events, administrators oversee the platform, and everyone can connect through real-time private messaging and a social follow network.

**Live demo:** https://eventhub-ca9u.onrender.com

---

## Table of Contents

- [Architecture](#architecture)
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Business Rules](#business-rules)
- [Security](#security)
- [Database & Seeding](#database--seeding)
- [Demo Accounts](#demo-accounts)
- [Running Locally](#running-locally)
- [Deployment](#deployment)
- [Unit Tests](#unit-tests)
- [Project Structure](#project-structure)

---

## Architecture

Five-layer solution with a strict one-directional dependency flow:

```
EventHub.Core → EventHub.Infrastructure → EventHub.Repositories → EventHub.Services → EventHub (Web)
```

| Layer | Responsibility |
|---|---|
| **EventHub.Core** | Domain entities, DTOs, enums, custom validation attributes, exception hierarchy |
| **EventHub.Infrastructure** | EF Core DbContext, Fluent API configurations, migrations, seeding, interceptors |
| **EventHub.Repositories** | Repository interfaces and implementations per entity |
| **EventHub.Services** | All business logic, AutoMapper profiles, Scrutor DI scanning |
| **EventHub (Web)** | Controllers, Razor views, ViewModels, SignalR hub, global filters |

### Key Architectural Decisions

- **Repository pattern** — one interface per entity, `AsNoTracking()` for reads, `AsSplitQuery()` for multi-collection includes
- **Scrutor assembly scanning** — services and repositories are auto-registered; no manual DI wiring
- **DomainExceptionFilter** — global `IExceptionFilter` that catches domain exceptions and converts them to user-friendly redirects with `TempData` messages instead of crashing
- **Exception-as-control-flow** — rich domain exception hierarchy (`DomainException` → `NotFoundException`, `ForbiddenException`, `ConflictException`, and ~20 specific subclasses) replaces boolean return values
- **Soft delete on events** — `IsDeleted` + `DeletedAt` with a global EF query filter; hard `Remove()` is never called
- **UTC normalization** — EF Core interceptor converts all `DateTime` values to UTC on save

---

## Features

### Events
- Browse, search, and filter by title, date range, location, and category
- Paginated event listings
- Create, edit, and soft-delete events (Organizer and Admin)
- Event detail page with full participant list
- Image upload with magic-byte format validation (JPG, PNG, GIF) — stored in Azure Blob Storage

### User Profiles
- Every registered user creates a personal profile (name, bio, phone, interests, location, profile image)
- Public profile page visible to other users

### Social Network
- Follow and unfollow other users
- Paginated followers and following lists

### Organizer System
- Users apply to become organizers
- Workflow: **Pending → Approved / Rejected**
- 7-day cooldown before reapplying after rejection
- Admins can approve, reject, or demote organizers back to User role
- Full request history with status filtering

### Real-Time Messaging (SignalR)
- Private 1-to-1 conversations between users
- Real-time message delivery via WebSocket
- Unread message indicators
- Conversation deduplication — only one conversation can exist per user pair, enforced at application and database level

### Admin Panel
- View and manage all events on the platform
- Manage organizer requests (approve / reject / demote)
- Full override access to edit or delete any event

---

## Tech Stack

| Technology | Version | Purpose |
|---|---|---|
| ASP.NET Core MVC | 10 | Web framework |
| Entity Framework Core | 10.0.2 | ORM, migrations, InMemory (tests) |
| ASP.NET Core Identity | 10.0.2 | Authentication and role management |
| SignalR | Built-in | Real-time WebSocket messaging |
| SQL Server / Azure SQL | — | Primary database |
| Azure Blob Storage | — | Image file storage |
| AutoMapper | 12.0.1 | DTO ↔ ViewModel mapping |
| Scrutor | 7.0.0 | Convention-based DI assembly scanning |
| Bootstrap 5 + Bootstrap Icons | — | Responsive UI |
| xUnit | 2.9.2 | Unit test framework |
| Moq | 4.20.72 | Mock objects for unit tests |
| FluentAssertions | 6.12.1 | Assertion library |
| C# | 13 | Language |

---

## Business Rules

### Event Join Validation

| Rule | Exception |
|---|---|
| Event must exist | `EventNotFoundException` |
| Event must not be expired | `EventExpiredException` |
| User cannot join twice | `UserAlreadyJoinedException` |
| Admin cannot join events | `AdminCannotJoinEventException` |
| Organizer cannot join own event | `OrganizerJoinOwnEventException` |
| Event must have capacity remaining | `EventFilledException` |

Concurrent joins at full capacity are handled atomically via a raw SQL statement with `UPDLOCK` and `HOLDLOCK` hints — preventing race conditions at the database level.

### Event Authorization

Only the event creator or an Admin can edit or delete an event. Enforced in `EventService.ValidateUserCanModifyEvent` — throws `ForbiddenOperationException` on violation.

### Organizer Application

- After rejection a **7-day cooldown** is enforced before reapplying
- Already-organizer and Admin users cannot submit new applications

### Messaging

- Conversation IDs are normalized by sorting the two user IDs alphabetically — `(A, B)` and `(B, A)` always resolve to the same record
- Unique database constraint on `(User1Id, User2Id)` backs this up at the database level

---

## Security

| Area | Implementation |
|---|---|
| CSRF | `[ValidateAntiForgeryToken]` on all POST actions |
| Open redirect | `returnUrl` validated with `Url.IsLocalUrl()` before any redirect |
| Image upload | Magic-byte (file signature) validation — extension alone is never trusted |
| Authorization | `[Authorize(Roles = "Admin")]` / `[Authorize(Roles = "Admin,Organizer")]` on all sensitive actions |
| SignalR | `[Authorize]` at the hub class level |
| Security stamp | Validated every 30 minutes — role changes take effect without requiring re-login |
| SQL injection | Prevented by EF Core parameterized queries; raw SQL uses parameters only |
| Custom error pages | Dedicated 404 and 500 pages — no stack traces or internal details exposed to users |

---

## Database & Seeding

The database is created, migrated, and seeded automatically on startup:

```
1. context.Database.MigrateAsync()   — apply pending migrations
2. RoleSeeder                        — create Admin / Organizer / User roles
3. IdentitySeeder                    — create demo user accounts
4. UserProfileSeeder                 — create profiles for seeded users
5. DataSeeder                        — seed locations from cities.json + categories
6. EventSeeder                       — create sample events with participants
7. InterestSeeder                    — seed interest tags for user profiles
```

### Connection String

Set in `EventHub/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=EventHub;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

---

## Demo Accounts

| Email | Password | Role |
|---|---|---|
| admin@eventhub.com | Admin123! | Admin — full platform access, cannot join events |
| organizer@eventhub.com | Admin123! | Organizer — creates and manages own events |
| user@eventhub.com | User123! | User — browses and joins events |

---

## Running Locally

### Prerequisites

- .NET 10 SDK
- SQL Server (LocalDB, Express, or full)

### Steps

```bash
# 1. Clone
git clone <repository-url>
cd EventHub

# 2. Set connection string
# Edit EventHub/appsettings.json → ConnectionStrings:DefaultConnection

# 3. (Optional) Set Azure Blob Storage credentials
# Edit appsettings.json → AzureBlobStorage:ConnectionString and ContainerName
# If omitted, image upload will not work but the app will still run

# 4. Run — database is created and seeded automatically
dotnet run --project EventHub
```

Navigate to `https://localhost:5001` (or the port shown in the terminal).

---

## Deployment

The live demo runs on:

- **Hosting:** Render (free tier) — https://eventhub-ca9u.onrender.com
- **Database:** Azure SQL (free tier)
- **File storage:** Azure Blob Storage (free tier)

The app listens on the `PORT` environment variable when set (Render requirement). A `Dockerfile` is included in the repository root.

---

## Unit Tests

Located in `EventHub.Tests/`. Run with:

```bash
dotnet test EventHub.Tests/EventHub.Tests.csproj
```

**74 tests** covering the five core service classes:

| Test Class | What is tested |
|---|---|
| `ParticipantServiceTests` | Join/leave validation — all 6 guard rules + success paths |
| `OrganizerServiceTests` | Apply / Approve / Reject / Demote + 7-day cooldown |
| `EventServiceTests` | CRUD, ownership checks, soft delete verification |
| `ConversationServiceTests` | ID normalization invariant (both orderings resolve to same pair) |
| `MessageServiceTests` | Send (content validation, trimming) + MarkAsRead |

Tests use **xUnit**, **Moq**, and **FluentAssertions** following the AAA pattern. EF Core `UseInMemoryDatabase` is used for services that require a `DbContext` directly.

---

## Project Structure

```
EventHub/
├── EventHub.Core/
│   ├── DTOs/                    # Data transfer objects per feature
│   ├── Enums/                   # Status enums (OrganizerRequestStatus, etc.)
│   ├── Exceptions/              # Domain exception hierarchy (~20 specific types)
│   ├── Models/                  # Entity models (Events, Users, Messaging, Social, Common)
│   └── ValidationAttributes/   # FutureDateAttribute, DateGreaterThanAttribute
│
├── EventHub.Infrastructure/
│   ├── Configurations/          # Fluent API IEntityTypeConfiguration per entity
│   ├── Data/
│   │   ├── ApplicationDbContext.cs
│   │   ├── Interceptors/        # SlowQueryInterceptor, UtcDateTimeInterceptor
│   │   └── Seed/                # Seeders (roles, users, events, locations, interests)
│   ├── Extensions/              # SeedExtensions, DatabaseExtensions
│   └── Identity/                # ApplicationUser
│
├── EventHub.Repositories/
│   ├── Interfaces/              # IEventRepository, IParticipantRepository, etc.
│   └── Implementations/         # Concrete repository classes
│
├── EventHub.Services/
│   ├── Interfaces/              # IEventService, IParticipantService, etc.
│   ├── Services/                # Business logic implementations
│   │   ├── Event/               # EventService, ParticipantService
│   │   ├── User/                # UserProfileService, OrganizerService, UserFollowService
│   │   ├── Messaging/           # ConversationService, MessageService
│   │   └── Common/              # BlobImageService, LocationService, CachedFormOptionsService
│   └── Mapping/                 # ServiceMappingProfile (AutoMapper)
│
├── EventHub/                    # ASP.NET Core MVC Web project
│   ├── Areas/Identity/          # Identity Razor Pages (login, register, logout)
│   ├── Controllers/             # MVC controllers (Events, Chat, UserProfile, Organizers, Admin)
│   ├── Filters/                 # DomainExceptionFilter, PerformanceMonitoringFilter
│   ├── Hubs/                    # ChatHub (SignalR)
│   ├── ViewModels/              # View-specific models
│   ├── Views/                   # Razor views + shared layouts and partials
│   └── wwwroot/                 # Static assets (CSS, JS, Bootstrap)
│
└── EventHub.Tests/              # xUnit test project
    └── Services/                # Service unit tests (5 test classes, 74 tests)
```

---

## Performance

- **`SlowQueryInterceptor`** — logs any EF Core query exceeding 500ms with full SQL
- **`PerformanceMonitoringFilter`** — logs action execution time (Error >500ms / Warning >200ms / Info otherwise)
- `AsNoTracking()` on all read-only queries
- `AsSplitQuery()` on queries with multiple collection includes
- `ProjectTo<T>()` for server-side column projection via AutoMapper
- DB indexes on: `Events.StartDate`, `EventParticipants(EventId, UserId)`, `Messages(ConversationId, CreatedAt)`, `UserFollows` (both directions)

---

## Author

**Toma Andreev**

[![GitHub](https://img.shields.io/badge/GitHub-Tomkata-black?logo=github)](https://github.com/Tomkata)
[![LinkedIn](https://img.shields.io/badge/LinkedIn-Toma_Andreev-0A66C2?logo=linkedin)](https://bg.linkedin.com/in/toma-andreev-05a7b6399?trk=people-guest_people_search-card)

**Contact:** tomaandreev12@gmail.com

---

*Developed as part of the SoftUni ASP.NET Advanced course — February 2026.*
