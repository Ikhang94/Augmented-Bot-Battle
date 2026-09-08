using BackendESP.Data;
using BackendESP.Models;
using BackendESP.Services;
using dotenv.net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.CodeAnalysis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var currentDir = Directory.GetCurrentDirectory();
var envFile = Path.Combine(currentDir, ".env");

if (!File.Exists(envFile))
{
    var projectRoot = FindProjectRoot(currentDir);
    if (projectRoot != null)
    {
        Directory.SetCurrentDirectory(projectRoot);
    }
}

DotEnv.Load(options: new DotEnvOptions(ignoreExceptions: true));
var enVars = DotEnv.Read();

var dbHost = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? (enVars.TryGetValue("POSTGRES_HOST", out var h) ? h : "localhost");
var dbName = enVars.ContainsKey("POSTGRES_DB") ? enVars["POSTGRES_DB"] : Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "testdb";
var dbUser = enVars.ContainsKey("POSTGRES_USER") ? enVars["POSTGRES_USER"] : Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "testuser";
var dbPassword = enVars.ContainsKey("POSTGRES_PASSWORD") ? enVars["POSTGRES_PASSWORD"] : Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
var JWT_key = enVars.ContainsKey("JWT_SECRET") ? enVars["JWT_SECRET"] : Environment.GetEnvironmentVariable("JWT_SECRET");
var JWT_issuer = enVars.ContainsKey("JwtSettings__Issuer") ? enVars["JwtSettings__Issuer"] : Environment.GetEnvironmentVariable("JwtSettings__Issuer") ?? "TestIssuer";
var JWT_audience = enVars.ContainsKey("JwtSettings__Audience") ? enVars["JwtSettings__Audience"] : Environment.GetEnvironmentVariable("JwtSettings__Audience") ?? "TestAudience";

if (string.IsNullOrEmpty(dbPassword))
    throw new InvalidOperationException("POSTGRES_PASSWORD environment variable is required");
if (string.IsNullOrEmpty(JWT_key))
    throw new InvalidOperationException("JWT_SECRET environment variable is required");

builder.Configuration["JwtSettings:Key"] = JWT_key;
builder.Configuration["JwtSettings:Issuer"] = JWT_issuer;
builder.Configuration["JwtSettings:Audience"] = JWT_audience;

builder.Services.AddDbContext<PostgresDbContext>(options =>
{
    options.UseNpgsql(
        $"Host={dbHost};" +
        $"Database={dbName};" +
        $"Username={dbUser};" +
        $"Password={dbPassword};" +
        $"SslMode=Require"
    );
});

builder.Services.AddScoped<IPlayerData, PlayerData>();
builder.Services.AddScoped<IRobotData, RobotData>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IGameSessionData, GameSessionData>();
builder.Services.AddScoped<ILeaderboardData, LeaderboardData>();

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWT_key))
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PostgresDbContext>();
    dbContext.Database.EnsureCreated();
}

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
});

app.MapGet("/", () => "Hello World!");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

static string? FindProjectRoot(string startDir)
{
    var dir = new DirectoryInfo(startDir);
    while (dir != null)
    {
        if (File.Exists(Path.Combine(dir.FullName, ".env")))
            return dir.FullName;
        dir = dir.Parent;
    }
    return null;
}

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

[ExcludeFromCodeCoverage]
public partial class Program { }
