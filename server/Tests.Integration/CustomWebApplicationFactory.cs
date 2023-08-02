using Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebApi;

namespace Tests.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _inMemoryDbInstanceName = Guid.NewGuid().ToString();
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.Remove(services.Single(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>)));
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase(_inMemoryDbInstanceName);
                options.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
            });

            using var scope = services.BuildServiceProvider().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<CustomWebApplicationFactory>>();
            try
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }
            catch (Exception e)
            {
                logger.LogError(e, "An error occurred creating the in-memory database. Error: {Message}", e.Message);
                throw;
            }
        });
    }
}