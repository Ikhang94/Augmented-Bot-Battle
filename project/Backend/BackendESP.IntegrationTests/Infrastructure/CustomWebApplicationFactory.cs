using BackendESP.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace BackendESP.IntegrationTests.Infrastructure;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = $"IntegrationTestDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // EF Core 8+ registers options via IDbContextOptionsConfiguration<T> — must remove all of them
            var optionsConfigDescriptors = services
                .Where(d => d.ServiceType == typeof(IDbContextOptionsConfiguration<PostgresDbContext>))
                .ToList();
            foreach (var d in optionsConfigDescriptors)
                services.Remove(d);

            var optionsDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<PostgresDbContext>));
            if (optionsDescriptor != null)
                services.Remove(optionsDescriptor);

            services.AddDbContext<PostgresDbContext>(options =>
                options.UseInMemoryDatabase(_dbName));
        });
    }

    public async Task<int> SeedPlayerAsync(string userName, string email, string password)
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PostgresDbContext>();
        var player = new Player { Username = userName, Email = email, Password = password };
        db.Players.Add(player);
        await db.SaveChangesAsync();
        return player.PlayerId;
    }

    public async Task<int> SeedRobotAsync(string name)
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PostgresDbContext>();
        var robot = new Robot { Name = name };
        db.Robots.Add(robot);
        await db.SaveChangesAsync();
        return robot.RobotId;
    }
}
