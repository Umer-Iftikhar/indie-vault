# IndieVault 🎮
 
A full-stack web platform where indie game developers can showcase their work, players can discover and download games, and admins can manage the platform. Built as two applications sharing one database: an ASP.NET Core MVC web app and a REST API with JWT authentication.
 
## 🌐 Live Demo
 
 ### MVC
**[indie-vault-production.up.railway.app](https://indie-vault-production.up.railway.app)**
 
> Note: First load may take 30 seconds if the server is waking up.
 
## ⚠️ Project Status
 
- Main branch: Active development
- Stable releases:
  - [Phase 8](https://github.com/Umer-Iftikhar/indie-vault/releases/tag/phase-8-complete)
  - [Phase 9](https://github.com/Umer-Iftikhar/indie-vault/releases/tag/phase-9-complete)
  - [Phase 10](https://github.com/Umer-Iftikhar/indie-vault/releases/tag/phase-10-complete)
---
 
## Projects
 
### IndieVault (MVC)
 
Server-rendered web application with Razor views, Bootstrap UI, and cookie-based authentication.

### Screenshots

### Discovery (Home & Search)

![Home](screenshots/home_screen.png)

### Game Details

![Details](screenshots/Game_Details.png)

### Developer Experience

![Game Upload](screenshots/Game_Upload.png)

### Admin Panel

![Admin Dashboard](screenshots/Admin_Dashboard.png)

---
 
### IndieVault.Api (REST API)
 
Stateless REST API with JWT authentication, designed to be consumed by any frontend client. Both projects share the same MySQL database.
 
 ### Screenshots

### All Endpoints
![EndPonts](screenshots/all_endpoints.png)

### Login Endpoint
![Login](screenshots/login_endpoint.png)

### Authorization
![Auth](screenshots/auth.png)

### Game Browse EndPoint
![Game Browse](screenshots/gamebrowse_endpoint.png)

---
 
## Roles
 
| Role | Permissions |
|------|------------|
| **Game Dev** | Upload, edit, and manage their own games |
| **Player** | Browse, wishlist, download, and review games |
| **Admin** | Manage genres, feature games, sync from RAWG, oversee platform |
 
---
 
## Features

### Both Projects
- Role-based authorization (Admin, GameDev, Player)
- ASP.NET Core Identity (user management, password hashing)
- Game upload with cover image and screenshots
- Advanced search, filtering, sorting, and pagination
- Wishlist functionality
- Review system with 1–5 star ratings
- Developer profiles with GitHub API integration
- RAWG API integration for importing real game data
- Admin dashboard with platform statistics
- Global exception handling with Serilog file logging
- Request logging middleware

### API Only
- JWT Bearer authentication with refresh token rotation
- Background sync service (RAWG games synced every 24 hours)
- Rate limiting on admin sync endpoint (1 request per 10 minutes)
- In-memory caching for form data with cache invalidation
- Swagger UI with JWT Bearer support

### MVC Only
- Cookie-based authentication via SignInManager
- Razor Views with Bootstrap 5 UI
- AJAX wishlist and live search (no page reload)
- Custom 404 and 500 error pages
---
 
## API Endpoints
 
### Auth — `api/account`
 
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/register` | None | Register as Player or GameDev |
| POST | `/login` | None | Login, returns access + refresh token |
| POST | `/refresh` | None | Refresh access token |
| POST | `/logout` | Required | Revoke refresh token |
 
### Profile — `api/profile`
 
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/{userId}` | Required | Get developer profile |
| PATCH | `/{userId}` | GameDev (owner) | Update GitHub username |
 
### Games — `api/game`
 
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `` | None | Browse games with filters and pagination |
| GET | `/{gameId}` | None | Get game details |
| GET | `/formdata` | GameDev | Get genres, engines, platforms, tags |
| GET | `/mine` | GameDev | Get logged-in dev's games |
| POST | `` | GameDev | Upload new game |
| PUT | `/{gameId}` | GameDev (owner) | Update game |
| DELETE | `/{gameId}` | GameDev (owner) | Delete game |
 
### Reviews
 
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `api/games/{gameId}/reviews` | Player | Create review |
| DELETE | `api/review/{reviewId}` | Player / Admin | Delete review |
 
### Wishlist — `api/wishlist`
 
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/{gameId}` | Player | Add game to wishlist |
| DELETE | `/{gameId}` | Player | Remove from wishlist |
| GET | `` | Player | View wishlist |
 
### Downloads — `api/download`
 
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/{gameId}` | Required | Download game |
| GET | `/history` | Required | View download history |
 
### Admin — `api/admin`
 
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/dashboard` | Admin | Platform statistics |
| POST | `/genres` | Admin | Create genre |
| DELETE | `/genres/{genreId}` | Admin | Delete genre |
| PATCH | `/games/{gameId}/feature` | Admin | Toggle featured status |
| POST | `/sync` | Admin | Sync games from RAWG (rate limited: 1/10 min) |
 
---
 
## Technologies
 
| Category | Technology |
|----------|-----------|
| Backend | ASP.NET Core 9, C# |
| ORM | Entity Framework Core (Code-First, Fluent API) |
| Queries | Dapper (read-heavy queries) |
| Database | MySQL |
| Auth (MVC) | ASP.NET Core Identity, Cookie Authentication |
| Auth (API) | JWT Bearer, Refresh Tokens |
| Frontend | Razor Views, Bootstrap 5, JavaScript (Fetch API) |
| API Docs | Swagger / Swashbuckle |
| Logging | Serilog (daily rolling files) |
| Caching | IMemoryCache |
| External APIs | GitHub REST API, RAWG Video Games Database API |
| Seed Data | Bogus |
| Testing | xUnit |
 
---
 
## Architecture
 
- Clean Architecture with separation of concerns
- Repository Pattern for abstracted data access
- Service Layer for business logic encapsulation
- DTOs for decoupling service layer from domain models
- Dependency Injection throughout (constructor injection)
- Hybrid data access: EF Core for writes, Dapper for complex reads
- EF Core Fluent API configurations in `Data/Configurations/`
- Custom middleware: global exception handling, request logging
- Role-based authorization throughout
---
 
## Dependency Flow
 
```
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
 
- .NET 10 SDK
- MySQL Server
- Visual Studio 2022
### MVC Project (IndieVault)
 
**1. Clone the repository**
 
```bash
git clone https://github.com/Umer-Iftikhar/indie-vault
```
 
**2. Restore dependencies**
 
```bash
dotnet restore
```
 
**3. Add connection string via User Secrets**
 
```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "server=localhost;database=IndieVault;user=root;password=your-password"
```
 
**4. Add RAWG API key via User Secrets**
 
```bash
dotnet user-secrets set "RawgApi:Key" "your-rawg-api-key"
```
 
Get a free API key from [rawg.io/apidocs](https://rawg.io/apidocs)
 
**5. Run migrations**
 
```powershell
Update-Database
```
 
**6. Run the application**
 
Database seeds automatically on first run in Development mode.
 
---
 
### API Project (IndieVault.Api)
 
**1. Add JWT settings and API keys via User Secrets**
 
```bash
dotnet user-secrets set "JwtSettings:SecretKey" "your-secret-key-minimum-32-characters"
dotnet user-secrets set "JwtSettings:Issuer" "indie-vault"
dotnet user-secrets set "JwtSettings:Audience" "indie-vault-users"
dotnet user-secrets set "RawgApi:Key" "your-rawg-api-key"
```
 
**2. Run the API**
 
The API shares the same database as the MVC project. Run MVC migrations first.
 
**3. Access Swagger UI**
 
```
http://localhost:{port}/swagger
```
 
Login via `POST /api/account/login`, copy the access token, click **Authorize** in Swagger, paste the token.
 
---
 
## Default Admin Account
 
```
Email:    admin@indiehub.com
Password: Password123!
```
 
---
 
## Testing
 
```bash
dotnet test
```
 
---
 
## Logging
 
Serilog with daily rolling log files stored in the `logs/` directory.
 
```
logs/
├── app-20260530.log
├── app-20260531.log
└── app-20260601.log
```
 
---
 
## Project Structure
 
```
IndieVault/                   # MVC Web Application
├── Controllers/
├── Models/
├── ViewModels/
├── DTOs/
├── Views/
├── Services/
│   ├── Interfaces/
│   └── Implementations/
│       └── ExternalApis/
├── Repositories/
│   ├── Interfaces/
│   └── Implementations/
├── Data/
│   ├── Configurations/
│   ├── AppDbContext.cs
│   ├── DatabaseSeeder.cs
│   └── Migrations/
├── Middleware/
├── Extensions/
├── Enums/
└── wwwroot/
 
IndieVault.Api/               # REST API
├── Controllers/
├── Models/
├── DTOs/
│   ├── Auth/
│   ├── Game/
│   ├── Review/
│   ├── Wishlist/
│   ├── Download/
│   ├── Admin/
│   ├── GitHub/
│   ├── Rawg/
│   └── Shared/
├── Services/
│   ├── Interfaces/
│   └── Implementations/
│       └── ExternalApis/
├── Repositories/
│   ├── Interfaces/
│   └── Implementations/
├── Data/
│   ├── Configurations/
│   ├── AppDbContext.cs
│   └── Migrations/
├── Middleware/
├── Extensions/
├── Enums/
└── Settings/
 
IndieVault.Tests/             # xUnit unit tests
```