
using IndieVault.Api.Data;
using IndieVault.Api.Extensions;
using IndieVault.Api.Models;
using IndieVault.Api.Repositories.Implementations;
using IndieVault.Api.Repositories.Interfaces;
using IndieVault.Api.Services.Implementations;
using IndieVault.Api.Services.Implementations.ExternalApis;
using IndieVault.Api.Services.Interfaces;
using IndieVault.Api.Services.Interfaces.ExternalApis;
using IndieVault.Api.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using System.Text;

Log.Logger = new LoggerConfiguration() // Configure Serilog to log at the Information level and above, with specific overrides for certain namespaces
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .WriteTo.Console()
    .WriteTo.File("logs/app.log", rollingInterval: RollingInterval.Day) // Write logs to a file with daily rolling intervals
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

// Register services
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IGameBrowseService, GameBrowseService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();
builder.Services.AddScoped<IDownloadService, DownloadService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddHttpClient<IGitHubService, GitHubService>(httpClient =>
{
    httpClient.BaseAddress = new Uri("https://api.github.com/");
    httpClient.DefaultRequestHeaders.Add("User-Agent", "IndieVault");
});

builder.Services.AddHttpClient<IRawgApiService, RawgApiService>(
    httpClient =>
    {
        httpClient.BaseAddress = new Uri("https://api.rawg.io/api/");
        httpClient.DefaultRequestHeaders.Add("User-Agent", "IndieVault");
    });

// Register repositories
builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();
builder.Services.AddScoped<IEngineRepository, EngineRepository>();
builder.Services.AddScoped<IScreenshotRepository, ScreenshotRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();
builder.Services.AddScoped<IAdminStatisticsRepository, AdminStatisticsRepository>();
builder.Services.AddScoped<IGameBrowseRepository, GameBrowseRepository>();
builder.Services.AddScoped<IDownloadRepository, DownloadRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();




var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

var serverVersion = new MySqlServerVersion(new Version(8, 0, 31));

builder.Services.AddDbContext<AppDbContext>
(
    options => options.UseMySql(connectionString, serverVersion,
    mySqlOptions => mySqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
);

builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredUniqueChars = 1;
    options.User.RequireUniqueEmail = true;

    // Optional: Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

// Add services to the container.

var jwtConfig = builder.Configuration.GetSection("JwtSettings").Get<JwtConfig>()
    ?? throw new InvalidOperationException("JWT configuration section 'JwtSettings' is missing or invalid.");

builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // Set the default authentication scheme to JWT Bearer
    // "Use JWT to authenticate every request"

    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // Set the default challenge scheme to JWT Bearer (used when authentication fails and a challenge is issued)
    // "When auth fails, return 401"
})
    .AddJwtBearer(options => // Add JWT Bearer authentication
    {
        options.TokenValidationParameters = new TokenValidationParameters // Configure the parameters for validating JWT tokens
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtConfig.Issuer,
            ValidAudience = jwtConfig.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey))
        };
    });

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseGlobalExceptionMiddleware();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseRequestLoggingMiddleware();

app.UseAuthorization();

app.MapControllers();

try
{
    app.Run();
}
catch (Exception ex)
{
    // Log any unhandled exceptions that occur during application startup or runtime
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    // Ensure that all logs are flushed and resources are released properly when the application is shutting down
    // This is important to ensure that all log entries are written to the configured sinks (e.g., console, file) before the application exits.
    Log.CloseAndFlush();
}
