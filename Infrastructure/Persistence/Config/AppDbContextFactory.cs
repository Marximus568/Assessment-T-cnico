using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Persistence.Config;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        // Load environment variables from .env
        DotNetEnv.Env.Load(Path.Combine(Directory.GetCurrentDirectory(), "..", ".env"));

        var connectionString = $"Server={Environment.GetEnvironmentVariable("DB_HOST")};" +
                               $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
                               $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                               $"User={Environment.GetEnvironmentVariable("DB_USER")};" +
                               $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};";

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        return new AppDbContext(optionsBuilder.Options);
    }
}
