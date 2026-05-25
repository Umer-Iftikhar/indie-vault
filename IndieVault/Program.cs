using IndieVault.Data;
using IndieVault.Extensions;
using IndieVault.Models;
using IndieVault.Services.Implementations;
using IndieVault.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
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

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
var serverVersion = new MySqlServerVersion(new Version(8, 0, 31));

builder.Services.AddDbContext<IndieVault.Data.AppDbContext>
(
    options => options.UseMySql(connectionString, serverVersion)
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging()
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
    options.User.AllowedUserNameCharacters = string.Empty; 
    options.User.RequireUniqueEmail = true;

    // Optional: Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<IndieVault.Data.AppDbContext>()
    .AddDefaultTokenProviders();



var app = builder.Build();

app.UseGlobalExceptionMiddleware();

app.UseStatusCodePagesWithRedirects("/Error/{0}");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await DatabaseSeeder.SeedAsync(app.Services);
}

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
