using Infrastructure.Persistence.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Persistence;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        var databaseOptions = new DatabaseOptions
        {
            ConnectionString =
                configuration["Database:ConnectionString"]
                ?? configuration["Database__ConnectionString"]
                ?? throw new InvalidOperationException(
                    "Database__ConnectionString is not configured.")
        };

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(databaseOptions.ConnectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}
