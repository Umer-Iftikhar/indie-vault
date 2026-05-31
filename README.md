# IndieVault 🎮

A full-stack ASP.NET Core MVC web application where indie game developers can showcase their work, players can discover and download games, and admins can manage the platform.

## ⚠️ Project Status

* Main branch: Active development (may contain incomplete or changing features)
* Stable releases:

  * [Phase 8](https://github.com/Umer-Iftikhar/indie-vault/releases/tag/phase-8-complete)
  * Phase 9 (see latest release)

---

## Roles

* **Game Dev** — Upload, edit, and manage their own games
* **Player** — Browse, wishlist, download, and review games
* **Admin** — Manage genres, feature games, sync games from RAWG, and oversee the platform

---

## Features

* Role-based authentication with ASP.NET Core Identity
* Game upload with cover image and screenshots
* Advanced search, filtering, sorting, and pagination
* Wishlist functionality with AJAX (no page reload)
* Review system with 1-5 star ratings
* Developer profiles with GitHub API integration
* RAWG API integration for importing real game data
* Admin dashboard with platform statistics and game sync
* Global exception handling with file logging
* Request logging middleware
* Custom 404 and 500 error pages

---

## Screenshots

### Discovery (Home & Search)

![Home](screenshots/home_screen.png)

### Game Details

![Details](screenshots/Game_Details.png)

### Player Interaction

![Empty Wishlist](screenshots/Empty_Wishlist.png)

![Wishlist](screenshots/Wishlist.png)

### Developer Experience

![Developer Profile](screenshots/Dev_Profile.png)

![Game Upload](screenshots/Game_Upload.png)

### Admin Panel

![Admin Dashboard](screenshots/Admin_Dashboard.png)

---

## Technologies

* ASP.NET Core 10 MVC
* Entity Framework Core (Code-First, Fluent API)
* Dapper (read-heavy queries)
* MySQL
* ASP.NET Core Identity
* Razor Views
* Bootstrap 5
* JavaScript (Fetch API / AJAX)
* GitHub REST API
* RAWG Video Games Database API
* Bogus (seed data)
* xUnit (unit tests)

---

## Architecture

* Clean Architecture with separation of concerns
* Repository Pattern for abstracted data access
* Service Layer for business logic encapsulation
* DTOs for decoupling service layer from domain models
* ViewModels for decoupling controllers from views
* Dependency Injection throughout (constructor injection)
* Hybrid data access:

  * EF Core for writes
  * Dapper for complex reads
* EF Core Fluent API configurations in `Data/Configurations/`
* External API integration:

  * GitHub API (developer profiles)
  * RAWG API (game data sync)
* Custom middleware:

  * Global exception handling
  * Request logging
* Role-based authorization throughout

---

## Dependency Flow

```text
HTTP Request
    ↓
Controller (thin — routing and response only)
    ↓
IService (injected via DI)
    ↓
Service (business logic, returns DTOs)
    ↓
IRepository (injected via DI)
    ↓
Repository (data access, returns entities)
    ↓
DbContext → MySQL
```

---

## Setup

### Prerequisites

* .NET 10 SDK
* MySQL Server
* Visual Studio 2022

### Steps

#### 1. Clone the repository

```bash
git clone https://github.com/Umer-Iftikhar/indie-vault
```

#### 2. Restore dependencies

```bash
dotnet restore
```

#### 3. Add connection string via User Secrets

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "server=localhost;database=IndieVault;user=root;password=your-password"
```

#### 4. Add RAWG API key via User Secrets

```bash
dotnet user-secrets set "RawgApi:Key" "your-rawg-api-key"
```

Get a free API key from:

https://rawg.io/apidocs

#### 5. Run migrations

Open **Package Manager Console** in Visual Studio:

```powershell
Update-Database
```

#### 6. Run the application

The database will be seeded automatically on first run in Development mode.

---

## Default Admin Account

```text
Email: admin@indiehub.com
Password: Password123!
```

---

## Testing

Unit tests are located in the `IndieVault.Tests` project.

Run tests through:

```text
Test → Run All Tests
```

or

```bash
dotnet test
```

---

## Notes

* GitHub API requires no key (public data, 60 requests/hour unauthenticated)
* RAWG API free tier allows 20,000 requests/month
* Game images are stored in:

```text
wwwroot/images/games/{gameId}/
```

* Error logs are written to:

```text
errors.log
```

in the project root.

---

## Project Structure

```text
IndieVault/
├── Controllers/          # HTTP request handling, thin actions
├── Models/               # Database entities (BaseEntity inherited by all)
├── ViewModels/           # Controller → View data transfer
├── DTOs/                 # Service layer data transfer objects
├── Views/                # Razor templates
├── Services/
│   ├── Interfaces/       # Service contracts
│   └── Implementations/  # Business logic
│       └── ExternalApis/ # GitHub and RAWG API services
├── Repositories/
│   ├── Interfaces/       # Repository contracts
│   └── Implementations/  # EF Core and Dapper data access
├── Data/
│   ├── Configurations/   # EF Core Fluent API entity configurations
│   ├── AppDbContext.cs
│   ├── DatabaseSeeder.cs
│   └── Migrations/
├── Middleware/           # Global exception handling, request logging
├── Extensions/           # Middleware registration extensions
├── Enums/                # Shared enumerations
└── wwwroot/              # Static files (CSS, JS, images)

IndieVault.Tests/         # xUnit unit tests
```
