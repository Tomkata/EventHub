# EventHub

A full-featured web-based event management platform built with ASP.NET Core MVC as a SoftUni course project. EventHub allows users to discover and join events, organizers to create and manage events, and administrators to oversee the platform. The application also includes a real-time private messaging system and a social follow network between users.

---

## Architecture

The project follows a strict 5-layer architecture with clear separation of concerns and one-directional dependency flow (`Core ← Infrastructure ← Repositories ← Services ← Web`).

### Project Structure

```
EventHub/
├── EventHub.Core/              # Domain models, DTOs, enums, validation attributes, custom exceptions
├── EventHub.Infrastructure/    # EF Core DbContext, Fluent API configurations, migrations, seeding, interceptors
├── EventHub.Repositories/      # Repository pattern (interfaces + implementations)
├── EventHub.Services/          # Business logic, AutoMapper profiles, DI registration
└── EventHub/                   # ASP.NET Core MVC — controllers, views, view models, SignalR hubs, filters
```

### Layer Responsibilities

**EventHub.Core**
Pure domain layer. Contains entity models (`Event`, `Category`, `Location`, `UserProfile`, `Conversation`, `Message`, `UserFollow`, `OrganizerRequest`), all DTOs, custom validation attributes (`FutureDateAttribute`, `DateGreaterThanAttribute`), and a rich domain exception hierarchy (`DomainException` → `NotFoundException`, `ForbiddenException`, `ConflictException`, etc.). Zero dependencies on other layers.

**EventHub.Infrastructure**
All EF Core concerns. Each entity has its own `IEntityTypeConfiguration<T>` class with full Fluent API configuration. Includes global UTC DateTime conversion, a `SlowQueryInterceptor` for performance monitoring, and an orchestrated data seeding pipeline.

**EventHub.Repositories**
Repository pattern with specific interfaces and implementations per entity. Uses `AsNoTracking()` for read queries, `AsSplitQuery()` for complex includes, and an atomic SQL implementation for safe concurrent event joins.

**EventHub.Services**
All business rules and validation live here. Services receive DTOs, validate them against domain rules, and delegate to repositories. Uses AutoMapper for entity ↔ DTO projections. Services and repositories are auto-registered via Scrutor convention scanning — no manual DI registration required.

**EventHub (Web)**
Thin MVC layer. Controllers delegate entirely to services and map results to view models via AutoMapper. Includes a `DomainExceptionFilter` for global graceful error handling, a `PerformanceMonitoringFilter` for action-level timing logs, and the SignalR `ChatHub` for real-time messaging.

---

## Features

### Events
- Browse, search, and filter events by title, date range, location, and category
- Paginated event listings
- Create, edit, and delete events (Organizer and Admin roles)
- Event detail page with full participant list
- Image upload with magic-byte format validation (JPG, PNG, GIF)

### User Profiles
- Every registered user can create a personal profile (name, bio, phone, profile image, interests, location)
- Public profile page visible to other users
- Edit profile with optional image replacement

### Social Network
- Follow and unfollow other users
- Paginated followers and following lists
- View other users' public profiles

### Organizer System
- Users can apply to become an organizer
- Application workflow: **Pending → Approved / Rejected**
- 7-day cooldown period before reapplying after rejection
- Admins can approve, reject, or demote organizers
- Full request history with status filtering

### Real-Time Messaging (SignalR)
- Private 1-to-1 conversations between users
- Real-time message delivery via WebSocket
- Unread message indicators on conversation list
- Conversations are deduplicated (only one conversation can exist between any two users)
- Message history with pagination

### Admin Panel
- View all events in the system
- Manage organizer requests (approve / reject / demote)
- Full override access to edit or delete any event

---

## Database Setup

The application uses SQL Server with Entity Framework Core. The database is initialized and seeded automatically on every startup.

### Initialization Sequence

```
1. context.Database.MigrateAsync()    — Apply all pending migrations
2. IdentitySeeder.SeedAsync(...)      — Create Admin/Organizer/User roles + demo accounts
3. DataSeeder.SeedAsync(...)          — Seed locations (cities.json) and categories
4. EventSeeder.SeedAsync(...)         — Create sample events with participants
5. InterestSeeder.SeedAsync(...)      — Seed interest tags for user profiles
```

### Connection String

Update the connection string in `EventHub/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=EventHub;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

---

## Demo Accounts

Three demo accounts are automatically created during seeding:

| Email | Password | Role | Notes |
|-------|----------|------|-------|
| admin@eventhub.com | Admin123! | Admin | Full access — modify any event, manage organizer requests. Cannot join events. |
| organizer@eventhub.com | Admin123! | Organizer | Create and manage own events. Can join events by other organizers. |
| user@eventhub.com | User123! | User | Browse, join, and leave events. Can apply to become an organizer. |

---

## Role-Based Authorization

### Admin
- View, edit, and delete **any** event regardless of who created it
- Approve, reject, and demote organizer requests
- Cannot join events (enforced as a business rule)

### Organizer
- Create, edit, and delete **their own** events
- Can join events created by other users
- Cannot join events they created themselves

### User
- Browse and join events (subject to capacity and eligibility rules)
- Apply to become an organizer
- Create a personal profile and follow other users
- Start private conversations with other users

---

## Business Rules

### Event Join Validation (`ParticipantService`)

| Rule | Exception Thrown |
|------|-----------------|
| Event must exist | `EventNotFoundException` |
| Event must not be expired | `EventExpiredException` |
| User cannot join the same event twice | `UserAlreadyJoinedException` |
| Admin users cannot join events | `AdminCannotJoinEventException` |
| Organizers cannot join their own events | `OrganizerJoinOwnEventException` |
| Event must not be at maximum capacity | `EventFilledException` |

**Concurrency safety**: The join operation uses an atomic SQL statement with `UPDLOCK` and `HOLDLOCK` hints to safely handle simultaneous join attempts at full capacity, preventing race conditions at the database level.

### Event Management Authorization

Only the event's creator (organizer) or an admin can edit or delete it. This is enforced in `EventService.ValidateUserCanModifyEvent`. Violation throws a `ForbiddenOperationException`.

### Organizer Application (`OrganizerService`)
- Applications move through: **Pending → Approved / Rejected**
- After a rejection, a **7-day cooldown** period must pass before reapplying
- Users who are already organizers or admins cannot submit new applications

### Messaging (`ConversationService`)
- Only one conversation can exist between any two users — enforced both in application code and via a unique database constraint on `(User1Id, User2Id)`
- Conversations are identified by normalizing the two user IDs into alphabetical order, so `(A, B)` and `(B, A)` always map to the same record
- Concurrent conversation creation (race condition) is handled gracefully by catching the unique constraint violation (`DbUpdateException`)

---

## Security

### CSRF Protection
All state-changing POST actions are decorated with `[ValidateAntiForgeryToken]`.

### Open Redirect Prevention
`returnUrl` parameters are validated with `Url.IsLocalUrl()` before use. The `DomainExceptionFilter` also validates redirect targets before executing them — no redirect to external domains is possible.

### Image Upload Validation
Images are validated by **file signature (magic bytes)**, not by file extension, preventing attackers from disguising malicious files as images:

```
PNG  → 89 50 4E 47 0D 0A 1A 0A
JPEG → FF D8 FF E0 / FF D8 FF E1
GIF  → 47 49 46 38
```

### Authorization
- All admin actions are gated with `[Authorize(Roles = "Admin")]`
- All organizer actions are gated with `[Authorize(Roles = "Admin,Organizer")]`
- The SignalR `ChatHub` requires authentication at the class level (`[Authorize]`)
- Both GET and POST endpoints for sensitive actions require the same role authorization

### Account Security
- Configurable password requirements, lockout thresholds, and cookie expiration via `appsettings.json`
- Security stamp validation interval of 10 seconds ensures role changes take effect immediately

---

## Performance

### Monitoring
- **`SlowQueryInterceptor`** — logs any EF Core query exceeding 500ms along with the full SQL statement
- **`PerformanceMonitoringFilter`** — logs every controller action's execution time at the appropriate level (Error > 500ms / Warning > 200ms / Info otherwise)

### Query Optimisation
- `AsNoTracking()` on all read-only queries
- `AsSplitQuery()` on queries with multiple collection includes (avoids Cartesian product explosion)
- `ProjectTo<T>()` with AutoMapper for server-side column projection (avoids loading full entity graphs)
- Indexes on: `Events.StartDate`, `EventParticipants(EventId, UserId)`, `Messages(ConversationId, CreatedAt)`, `UserFollows(FollowingId, CreatedAt)`, `UserFollows(FollowerId, CreatedAt)`

---

## Technologies Used

| Technology | Version | Purpose |
|-----------|---------|---------|
| ASP.NET Core MVC | 10 | Web framework |
| Entity Framework Core | 10.0.2 | ORM and database migrations |
| ASP.NET Core Identity | 10.0.2 | Authentication and role-based authorization |
| SignalR | Built-in | Real-time WebSocket messaging |
| SQL Server | — | Primary database |
| AutoMapper | 12.0.1 | DTO / ViewModel mapping |
| Scrutor | 7.0.0 | Convention-based DI scanning |
| Bootstrap 5 | — | UI framework |
| C# | 13 | Language |

### Design Patterns Applied
- **Repository Pattern** — abstraction over data access
- **Service Layer Pattern** — business logic encapsulation
- **DTO Pattern** — clean data transfer between layers
- **Exception-as-control-flow** — rich domain exception hierarchy handled by a global `IExceptionFilter`
- **Dependency Injection** — built-in ASP.NET Core DI with Scrutor assembly scanning

---

## Running the Application

### Prerequisites
- .NET 10 SDK
- SQL Server (LocalDB, SQL Server Express, or full SQL Server)
- Visual Studio 2022 / JetBrains Rider / VS Code

### Steps

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd EventHub
   ```

2. **Configure the connection string**
   Edit `EventHub/appsettings.json` and set your SQL Server connection string.

3. **Run the application**
   The database is created, migrated, and seeded automatically on first run.

   Using .NET CLI:
   ```bash
   dotnet run --project EventHub
   ```

   Using Visual Studio:
   - Open the solution file (`.slnx`)
   - Press **F5** or click **Run**

4. **Open in browser**
   Navigate to `https://localhost:5001` (or the port shown in your terminal).

5. **Sign in with a demo account**
   Use any account from the [Demo Accounts](#demo-accounts) section above.

---

## Future Improvements

| # | Improvement | Priority |
|---|-------------|----------|
| 1 | Unit tests for the service layer (join rules, organizer cooldown, conversation logic) | High |
| 2 | `IMemoryCache` for static reference data (categories, locations) | High |
| 3 | Real-time notifications hub (follow events, organizer request outcomes) | Medium |
| 4 | Admin dashboard with platform statistics (total events, users, pending requests) | Medium |
| 5 | Soft delete for events (preserve history, prevent data loss) | Medium |
| 6 | Rate limiting on chat messages to prevent spam | Medium |
| 7 | Generic base repository to reduce duplication across repositories | Low |
| 8 | Blob storage for uploaded images instead of local file system | Low |
| 9 | Email notifications for organizer status changes and event reminders | Low |
| 10 | Waitlist system for events at maximum capacity | Low |

---

**Academic Project Context**: Developed as part of the SoftUni ASP.NET Advanced course to demonstrate understanding of layered ASP.NET Core MVC architecture, Entity Framework Core, real-time communication with SignalR, and secure web application development practices.

---

## 👨‍💻 Author

**Toma Andreev**

[![GitHub](https://img.shields.io/badge/GitHub-Tomkata-black?logo=github)](https://github.com/Tomkata)
[![LinkedIn](https://img.shields.io/badge/LinkedIn-Toma_Andreev-0A66C2?logo=linkedin)](https://bg.linkedin.com/in/toma-andreev-05a7b6399?trk=people-guest_people_search-card)
[![Instagram](https://img.shields.io/badge/Instagram-toma__andreev-purple?logo=instagram)](https://www.instagram.com/toma_andreev/)

---

## 📧 Contact

- Email: tomaandreev12@gmail.com
- LinkedIn: https://www.linkedin.com/in/toma-andreev-05a7b6399/
