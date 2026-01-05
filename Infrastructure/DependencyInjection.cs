using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence.Config;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Load environment variables from .env
        DotNetEnv.Env.TraversePath().Load();

        var host = Environment.GetEnvironmentVariable("DB_HOST");
        var port = Environment.GetEnvironmentVariable("DB_PORT");
        var name = Environment.GetEnvironmentVariable("DB_NAME");
        var user = Environment.GetEnvironmentVariable("DB_USER");
        var pass = Environment.GetEnvironmentVariable("DB_PASSWORD");

        if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(name))
        {
            throw new InvalidOperationException("Database environment variables (DB_HOST, DB_NAME, etc.) are not configured in .env");
        }

        var connectionString = $"Server={host};Port={port};Database={name};User={user};Password={pass};";
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 21));

        // Business DbContext
        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(connectionString, serverVersion));

        // Auth DbContext (separate database context for Identity)
        services.AddDbContext<AuthDbContext>(options =>
            options.UseMySql(connectionString, serverVersion));

        // Identity Configuration
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            // Password settings
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;

            // User settings
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<AuthDbContext>()
        .AddDefaultTokenProviders();

        // Repositories
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<ILessonRepository, LessonRepository>();

        return services;
    }
}
