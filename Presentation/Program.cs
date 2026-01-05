using Infrastructure;
using Application;
using Microsoft.OpenApi.Models;

// Load environment variables from .env (traverse up to find it)
DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);


// Ensure environment variables are loaded into Configuration
builder.Configuration.AddEnvironmentVariables();

// --------------------------------------------------
// Add services
// --------------------------------------------------

// Infrastructure & Application DI
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

// Controllers
builder.Services.AddControllers();

// CORS - Allow from any origin
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var secretKey = builder.Configuration["JwtSettings:Secret"] 
                    ?? builder.Configuration["JWTSETTINGS__SECRET"]
                    ?? Environment.GetEnvironmentVariable("JWTSETTINGS__SECRET")
                    ?? throw new InvalidOperationException("JWT Secret not configured. Ensure JWTSETTINGS__SECRET is set in .env");
    
    var issuer = builder.Configuration["JwtSettings:Issuer"] 
                 ?? builder.Configuration["JWTSETTINGS__ISSUER"]
                 ?? Environment.GetEnvironmentVariable("JWTSETTINGS__ISSUER");

    var audience = builder.Configuration["JwtSettings:Audience"] 
                   ?? builder.Configuration["JWTSETTINGS__AUDIENCE"]
                   ?? Environment.GetEnvironmentVariable("JWTSETTINGS__AUDIENCE");
    
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddAuthorization();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Courses API",
        Version = "v1",
        Description = "Courses management module for the educational system with Identity and JWT support."
    });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\""
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
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

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();