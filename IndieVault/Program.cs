using IndieVault.Data;
using IndieVault.Extensions;
using IndieVault.Models;
using IndieVault.Repositories.Implementations;
using IndieVault.Repositories.Interfaces;
using IndieVault.Services.Implementations;
using IndieVault.Services.Implementations.ExternalApis;
using IndieVault.Services.Interfaces;
using IndieVault.Services.Interfaces.ExternalApis;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration() // Configure Serilog to log at the Information level and above, with specific overrides for certain namespaces
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .WriteTo.Console()
    .WriteTo.File("logs/app.log", rollingInterval: RollingInterval.Day) // Write logs to a file with daily rolling intervals
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(); //  it replaces the default ASP.NET Core logging provider with Serilog globally. 

builder.Services.AddControllersWithViews();

// Register services
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IGameBrowseService, GameBrowseService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();
builder.Services.AddScoped<IDownloadService, DownloadService>();
builder.Services.AddScoped<IAdminService, AdminService>();

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

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

var serverVersion = new MySqlServerVersion(new Version(8, 0, 31));

builder.Services.AddDbContext<IndieVault.Data.AppDbContext>
(
    options => options.UseMySql(connectionString, serverVersion,
    mySqlOptions => mySqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
);

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings
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
.AddEntityFrameworkStores<IndieVault.Data.AppDbContext>()
    .AddDefaultTokenProviders();



var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Show detailed error pages in development
}
else
{
    app.UseGlobalExceptionMiddleware(); // Custom global exception handling middleware
    app.UseHsts(); // Use HTTP Strict Transport Security in production
}


app.UseStatusCodePagesWithRedirects("/Error/{0}"); // Handle 404 and other status codes by redirecting to a custom error page

if (app.Environment.IsDevelopment())
{
    await DatabaseSeeder.SeedAsync(app.Services);
}

app.UseRouting();

app.UseAuthentication();

app.UseRequestLoggingMiddleware();

app.UseAuthorization();

app.MapStaticAssets(); 

app.MapControllerRoute( 
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


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
