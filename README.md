# EventHub

A web-based event management system built with ASP.NET Core MVC as a SoftUni course project. The application allows users to browse events, organizers to create and manage events, and administrators to oversee the platform.

## Architecture

The project follows a layered architecture pattern with clear separation of concerns:

### Project Structure

```
EventHub/
├── EventHub.Core/              # Domain models, DTOs, enums, validation attributes, exceptions
├── EventHub.Infrastructure/    # Data access, EF Core configurations, migrations, Identity setup
├── EventHub.Repositories/      # Repository pattern implementation (interfaces + concrete classes)
├── EventHub.Services/          # Business logic layer with validation and rules
└── EventHub/                   # ASP.NET Core MVC web layer (controllers, views, areas)
```

### Layer Responsibilities

**EventHub.Core**  
Contains domain models (Event, Category, Location, OrganizerRequest), DTOs for data transfer, custom validation attributes, and domain-specific exceptions. This layer has no dependencies on other layers.

**EventHub.Infrastructure**  
Handles database context (ApplicationDbContext), Entity Framework Core configurations, migrations, and data seeding. Includes ASP.NET Core Identity setup with the ApplicationUser model.

**EventHub.Repositories**  
Implements the repository pattern with generic base repository and specific repositories for each entity. Provides abstraction over data access operations.

**EventHub.Services**  
Contains business logic and enforces business rules. Services validate operations before delegating to repositories (e.g., ParticipantService validates join/leave operations, EventService validates event ownership).

**EventHub (Web)**  
MVC layer with controllers, views, view models, and Razor Pages for Identity UI. Handles HTTP requests, user input validation, and presentation logic.

## Database Setup

The application uses SQL Server and Entity Framework Core with automatic initialization on startup.

### Initialization Sequence

The database is initialized automatically when the application starts (`Program.cs`, lines 71-84):

```csharp
1. await context.Database.MigrateAsync();           // Apply pending migrations
2. await IdentitySeeder.SeedAsync(...);             // Create roles and demo users
3. await DataSeeder.SeedAsync(context);             // Seed locations from cities.json
4. await EventSeeder.SeedAsync(context, ...);       // Create sample events
```

This approach ensures the database is always in a consistent state with test data available for development and evaluation.

### Connection String

Update the connection string in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EventHub;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

## Demo Accounts

Three demo accounts are automatically created during seeding:

| Email | Password | Role | Permissions |
|-------|----------|------|-------------|
| admin@eventhub.com | Admin123! | Admin | Full access, can modify any event, manage requests for organizer, cannot join events |
| organizer@eventhub.com | Admin123! | Organizer | Can create/manage own events, can join other events |
| user@eventhub.com | User123! | User | Can browse, can join events |

**Note**: Admin accounts cannot join events. Organizers cannot join events they created.

## Role-Based Authorization

The application implements three distinct user roles:

### Admin
- Can view, edit, and delete any event regardless of ownership
- Can manage organizer applications (approve/reject)
- Cannot join events (business rule enforcement)
- Has unrestricted access to all system features

### Organizer
- Can create new events
- Can edit and delete their own events only
- Can join events created by other organizers
- Cannot modify events they don't own

### User
- Can browse all published events
- Can join and leave events (subject to capacity and eligibility rules)
- Can apply to become an organizer
- Read-only access to event management features

## Business Rules

### Event Join Validation

The `ParticipantService` enforces the following rules when a user attempts to join an event:

1. **Event Existence**: Event must exist in the database
2. **Event Status**: Event must not be expired (EndDate must be in the future)
3. **Duplicate Prevention**: User cannot join the same event twice
4. **Admin Restriction**: Admin users cannot join any events
5. **Organizer Restriction**: Organizers cannot join events they created
6. **Capacity Check**: Event must not be at maximum capacity (MaxParticipants)

**Known Issue**: The current implementation has a potential race condition when multiple users join simultaneously near capacity. This is acknowledged in the code comments (`ParticipantService.cs`, lines 18-21) and could be addressed with optimistic concurrency control in a production environment.

### Event Leave Validation

Users can only leave events they have previously joined. The service verifies participation before allowing the leave operation.

### Event Management Authorization

The `EventService` includes ownership validation (`ValidateUserCanModifyEvent` method):

- Only the event creator (organizer) or an admin can edit/delete an event
- Throws `UnauthorizedAccessException` if a user attempts to modify an event they don't own
- Validates that the organizer exists in the system when creating events

### Organizer Application System

Users can apply to become organizers through the `OrganizerRequest` model:

- Applications have three states: Pending, Approved, Rejected
- Includes a cooldown mechanism (LastRejectedAt timestamp)
- Admins can review applications and add notes during approval/rejection
- Upon approval, the user is granted the Organizer role

## Security Considerations

### Image Upload Validation

The `ImageService` implements magic-byte validation to prevent malicious file uploads:

```csharp
// Validates actual file content, not just the extension
private bool IsValidImage(byte[] fileBytes)
{
    if (fileBytes.Length < 8) return false;
    
    // Check PNG signature: 89 50 4E 47 0D 0A 1A 0A
    if (fileBytes[0] == 0x89 && fileBytes[1] == 0x50 && ...)
        return true;
    
    // Check JPEG signatures: FF D8 FF E0/E1
    if (fileBytes[0] == 0xFF && fileBytes[1] == 0xD8 && ...)
        return true;
    
    return false;
}
```

This prevents attackers from uploading malicious files disguised as images by renaming them with image extensions.

### ASP.NET Core Identity Configuration

Identity settings are configurable through `appsettings.json` and applied in `Program.cs` (lines 38-61):

**Password Requirements**
- Configurable minimum length
- Optional digit, uppercase, and lowercase requirements

**Account Lockout**
- Maximum failed login attempts
- Lockout duration in minutes

**Cookie Configuration**
- Sliding expiration window
- Configurable session timeout
- Security stamp validation interval (set to 10 seconds)

### Authorization Enforcement

Controllers use the `[Authorize]` attribute with role requirements:
- Event creation/editing requires Organizer or Admin role
- Admin dashboard requires Admin role
- Organizer applications use role-based access control

## Running the Application

### Prerequisites

- .NET 8.0 SDK or later
- SQL Server (LocalDB, SQL Server Express, or full SQL Server)
- Visual Studio 2022 or JetBrains Rider (optional but recommended)

### Steps

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd EventHub
   ```

2. **Update the connection string**  
   Edit `EventHub/appsettings.json` and set your SQL Server connection string.

3. **Run the application**  
   The database will be created and seeded automatically on first run.
   
   Using .NET CLI:
   ```bash
   dotnet run --project EventHub
   ```
   
   Using Visual Studio:
   - Open the solution file
   - Press F5 or click Run

4. **Access the application**  
   Navigate to `https://localhost:5001` (or the port shown in your terminal)

5. **Sign in with a demo account**  
   Use one of the accounts listed in the Demo Accounts section above.

## Technologies Used

- **ASP.NET Core 10 MVC** - Web framework
- **Entity Framework Core** - ORM for data access
- **ASP.NET Core Identity** - Authentication and authorization
- **SQL Server** - Database (LocalDB for development)
- **Bootstrap 5** - UI framework
- **Razor Pages** - Identity UI scaffolding
- **C#** - Programming language

### Design Patterns

- **Repository Pattern** - Abstraction over data access
- **Service Layer Pattern** - Business logic encapsulation
- **Dependency Injection** - Built-in ASP.NET Core DI container
- **Unit of Work** (implicit through DbContext)

## Future Improvements

Based on the current implementation, realistic improvements for a production system would include:

1. **Concurrency Handling**  
   Implement optimistic concurrency control for event joins using row versioning to prevent race conditions when multiple users join near capacity.

2. **Image Handling Enhancements**
   - Add file size limits (currently unlimited)
   - Implement image resizing to standardize dimensions
   - Add support for WebP format
   - Store images in blob storage instead of file system

3. **Event Notifications**  
   Email notifications for event updates, cancellations, and reminders using a background job processor (Hangfire or similar).

4. **Search and Filtering**
   - Full-text search for events
   - Advanced filtering by date range, category, location
   - Pagination for event lists (currently loads all events)

5. **Audit Logging**  
   Track who created, modified, or deleted events for accountability and debugging.

6. **Unit Testing**  
   Comprehensive test coverage for business rules in the service layer, especially join/leave validation logic.

7. **Caching**  
   Implement output caching for frequently accessed data (categories, locations) to reduce database load.

8. **Rate Limiting**  
   Prevent abuse by limiting the number of event joins/leaves per user per time period.

9. **Soft Deletes**  
   Instead of hard deleting events, mark them as deleted to preserve historical data.

10. **Event Capacity Management**  
    Implement a waitlist system for events that reach capacity.

11. **Custom Accounts and chat**  
     User create accounts with information and can invite each other and intruduce them.
    
12. **NLP(Natural language processing suggestions)**  
     On base discription and interests on users. The proiles will be suggested each other.
---

**Academic Project Context**: This project was developed as part of a SoftUni course to demonstrate understanding of ASP.NET Core MVC, Entity Framework Core, layered architecture, and secure web application development practices.



## 👨‍💻 Author

**Toma Andreev**  

[![GitHub](https://img.shields.io/badge/GitHub-Tomkata-black?logo=github)](https://github.com/Tomkata)
[![LinkedIn](https://img.shields.io/badge/LinkedIn-Toma_Andreev-0A66C2?logo=linkedin)](https://bg.linkedin.com/in/toma-andreev-05a7b6399?trk=people-guest_people_search-card)
[![Instagram](https://img.shields.io/badge/Instagram-toma__andreev-purple?logo=instagram)](https://www.instagram.com/toma_andreev/)



---

## 📧 Contact

For questions, suggestions, or collaboration:
- Email: tomaandreev12@gmail.com
- LinkedIn: https://www.linkedin.com/in/toma-andreev-05a7b6399/

---
