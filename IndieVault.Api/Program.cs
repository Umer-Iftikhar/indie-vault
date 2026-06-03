
using IndieVault.Api.Data;
using IndieVault.Api.Models;
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

var jwtConfig = builder.Configuration.GetSection("JwtSettings").Get<JwtConfig>();
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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
