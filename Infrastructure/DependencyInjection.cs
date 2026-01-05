using Application.Interfaces;
using Infrastructure.Persistence.Config;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        DotNetEnv.Env.Load();

        var connectionString = $"Server={Environment.GetEnvironmentVariable("DB_HOST")};" +
                               $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
                               $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                               $"User={Environment.GetEnvironmentVariable("DB_USER")};" +
                               $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};";

        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<ILessonRepository, LessonRepository>();

        return services;
    }
}
