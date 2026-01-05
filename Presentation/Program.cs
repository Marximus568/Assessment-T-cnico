using Infrastructure;
using Application;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------
// Add services
// --------------------------------------------------

// Infrastructure & Application DI
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Courses API",
        Version = "v1",
        Description = "API for managing courses and lessons"
    });
});

// --------------------------------------------------
// Build app
// --------------------------------------------------

var app = builder.Build();

// --------------------------------------------------
// Middleware pipeline
// --------------------------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Courses API v1");
        options.RoutePrefix = string.Empty; // Swagger en raíz (/)
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();