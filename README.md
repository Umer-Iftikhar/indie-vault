IndieVault 🎮
A full-stack ASP.NET Core MVC web application where indie game developers can showcase their work, players can discover and download games, and admins can manage the platform.
⚠️ Project Status

Main branch: Active development (may contain incomplete or changing features)
Stable releases: Phase 8 | Phase 9 (see latest release)

Roles

Game Dev — upload, edit, and manage their own games
Player — browse, wishlist, download, and review games
Admin — manage genres, feature games, sync games from RAWG, and oversee the platform

Features

Role-based authentication with ASP.NET Core Identity
Game upload with cover image and screenshots
Advanced search, filtering, sorting, and pagination
Wishlist functionality with AJAX (no page reload)
Review system with 1-5 star ratings
Developer profiles with GitHub API integration
RAWG API integration for importing real game data
Admin dashboard with platform statistics and game sync
Global exception handling with file logging
Request logging middleware
Custom 404 and 500 error pages

Screenshots
Discovery (Home & Search)
Show Image
Game Details
Show Image
Player Interaction
Show Image
Show Image
Developer Experience
Show Image
Show Image
Admin Panel
Show Image
Technologies

ASP.NET Core 10 MVC
Entity Framework Core (Code-First, Fluent API)
Dapper (read-heavy queries)
MySQL
ASP.NET Core Identity
Razor Views
Bootstrap 5
JavaScript (Fetch API / AJAX)
GitHub REST API
RAWG Video Games Database API
Bogus (seed data)
xUnit (unit tests)

Architecture

Clean Architecture with separation of concerns
Repository Pattern for abstracted data access
Service Layer for business logic encapsulation
DTOs for decoupling service layer from domain models
ViewModels for decoupling controllers from views
Dependency Injection throughout (constructor injection)
Hybrid data access: EF Core for writes, Dapper for complex reads
EF Core Fluent API configurations in Data/Configurations/
External API integration: GitHub API (developer profiles), RAWG API (game data sync)
Custom middleware: global exception handling and request logging
Role-based authorization throughout

Dependency Flow
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
Setup
Prerequisites

.NET 10 SDK
MySQL Server
Visual Studio 2022

Steps

Clone the repository:

   git clone https://github.com/Umer-Iftikhar/indie-vault

Restore dependencies:

   dotnet restore

Add connection string via user secrets:

   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "server=localhost;database=IndieVault;user=root;password=your-password"

Add RAWG API key via user secrets:

   dotnet user-secrets set "RawgApi:Key" "your-rawg-api-key"
Get a free key at https://rawg.io/apidocs

Run migrations:
Open Package Manager Console in Visual Studio:

   Update-Database

Run the application.
The database will be seeded automatically on first run in Development.

Default Admin Account

Email: admin@indiehub.com
Password: Password123!

Testing
Unit tests are in the IndieVault.Tests project.
Run via: Test → Run All Tests in Visual Studio
Notes

GitHub API requires no key (public data, 60 requests/hour unauthenticated)
RAWG API free tier allows 20,000 requests/month
Game images are stored in wwwroot/images/games/{gameId}/
Error logs are written to errors.log in the project root

Project Structure
IndieVault/
├── Controllers/          — HTTP request handling, thin actions
├── Models/               — Database entities (BaseEntity inherited by all)
├── ViewModels/           — Controller → View data transfer
├── DTOs/                 — Service layer data transfer objects
├── Views/                — Razor templates
├── Services/
│   ├── Interfaces/       — Service contracts
│   └── Implementations/  — Business logic
│       └── ExternalApis/ — GitHub and RAWG API services
├── Repositories/
│   ├── Interfaces/       — Repository contracts
│   └── Implementations/  — EF Core and Dapper data access
├── Data/
│   ├── Configurations/   — EF Core Fluent API entity configurations
│   ├── AppDbContext.cs
│   ├── DatabaseSeeder.cs
│   └── Migrations/
├── Middleware/           — Global exception handling, request logging
├── Extensions/           — Middleware registration extensions
├── Enums/                — Shared enumerations
└── wwwroot/              — Static files (CSS, JS, images)

IndieVault.Tests/         — xUnit unit tests