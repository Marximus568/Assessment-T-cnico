# Backend Architecture - Assessment

## 📋 General Description

A course management system built with **ASP.NET Core 8** following a **Clean Architecture** layered pattern with design patterns like **CQRS** (Command Query Responsibility Segregation) and **Repository Pattern**.

The application is designed to handle:
- Course management (create, publish, unpublish, delete)
- Lesson management within courses
- Authentication and authorization with JWT
- Automatic DTO mapping with AutoMapper
- MySQL database with Entity Framework Core

---

## 🏗️ Layered Architecture

### 1. **Domain** (Domain Layer)
Contains pure business logic independent of frameworks.

**Folders:**
- **`Entities/`** - Main domain entities
  - `ApplicationUser.cs` - Application user (extends IdentityUser)
  - `Course.cs` - Course entity
  - `Lesson.cs` - Lesson entity

- **`Enums/`** - Domain enumerations
  - `CourseStatus.cs` - Course states (Draft, Published, Archived)

- **`Interfaces/`** - Contracts to be implemented by other layers

**Responsibilities:**
- Define entities and their business rules
- Establish contracts (interfaces) for repositories
- No external dependencies (no EF, no ASP.NET Core)

---

### 2. **Application** (Application Layer)
Orchestrates business logic and implements use cases.

**Folders:**

- **`DTOs/`** - Data Transfer Objects for data transfer
  - `CourseDto.cs` - Course DTO
  - `LessonDto.cs` - Lesson DTO
  - `PagedResult.cs` - Generic paginated result
  - `Auth/` - Authentication DTOs
  - `Courses/` - Course-specific DTOs (e.g., CourseSummaryDto)

- **`Interfaces/`** - Service contract interfaces
  - `IAuthService.cs` - Authentication service interface
  - `ICourseService.cs` - Course service interface
  - `ILessonService.cs` - Lesson service interface
  - `ICourseRepository.cs` - Course repository interface
  - `ILessonRepository.cs` - Lesson repository interface

- **`Services/`** - Application service implementations
  - `AuthService.cs` - Registration and login logic
  - `CourseService.cs` - Course operation logic
  - `LessonService.cs` - Lesson operation logic

- **`Mappings/`** - AutoMapper mapping configuration
  - `MappingProfile.cs` - Mapping profiles between entities and DTOs

- **`UseCases/`** - Use cases organized by module
  - `Courses/CreateCourse/` - CQRS command for creating course
  - `Lessons/CreateLesson/` - CQRS command for creating lesson

- **`Handlers/`** - CQRS command and query handlers (prepared structure)

**Responsibilities:**
- Implement business logic
- Coordinate between services
- Transform entities to DTOs
- Handle business exceptions

**Dependencies:**
- Domain (reference to entities)
- AutoMapper
- Microsoft.Extensions.DependencyInjection

---

### 3. **Infrastructure** (Infrastructure Layer)
Implements technical details: data access, persistence, external services.

**Folders:**

- **`Persistence/`** - Database configuration
  - **`Config/`** - Database context configurations
    - `AppDbContext.cs` - Main context (Courses and Lessons)
    - `AuthDbContext.cs` - Identity and authentication context

- **`Repositories/`** - Repository pattern implementation
  - `CourseRepository.cs` - Course repository
  - `LessonRepository.cs` - Lesson repository
  - Abstract base repositories

- **`Services/`** - Infrastructure services
  - Email, notification services, etc. (prepared structure)

- **`Migrations/`** - Entity Framework Core migrations
  - Timestamped versioning
  - Schema snapshots

**Responsibilities:**
- Implement data access (repositories)
- Configure Entity Framework Core
- Manage database migrations
- Authentication and security (Identity)

**Dependencies:**
- Domain and Interfaces
- Entity Framework Core
- MySQL connector
- Microsoft.AspNetCore.Identity

---

### 4. **Presentation** (Presentation Layer)
Exposes the REST API and handles HTTP requests.

**Components:**

- **`Program.cs`** - Main application configuration
  - Dependency injection (DI Container)
  - Middleware pipeline
  - JWT authentication configuration
  - CORS
  - Swagger/OpenAPI

- **`Controllers/`** - REST controllers
  - `AuthController.cs` - Authentication endpoints
  - `CourseController.cs` - Course endpoints
  - `LessonController.cs` - Lesson endpoints

- **`Properties/launchSettings.json`** - Execution configuration
  - Port (5129)
  - Development profile

**Responsibilities:**
- Receive and validate HTTP requests
- Call application services
- Return HTTP responses with appropriate status codes
- Document endpoints with Swagger

**Dependencies:**
- Application and Infrastructure
- ASP.NET Core
- Swagger/Swashbuckle

---

## 🔄 Request Flow

```
HTTP Client
    ↓
Middleware Pipeline (CORS, Auth, etc.)
    ↓
Controller (Presentation)
    ↓
Application Service (Application)
    ↓
Repository (Infrastructure)
    ↓
Entity Framework Core + MySQL
    ↓
Database
```

### Example: Creating a Course

1. **Client** sends POST to `/api/v1/course` with `{ "title": "Advanced C#" }`
2. **CourseController** receives the request
3. **CourseService** executes `CreateCourseAsync(title)`
4. **Course** entity is created with Draft status
5. **CourseRepository** persists the entity to DB
6. Returns created course ID to client

---

## 🔐 Security

### Authentication
- **JWT (JSON Web Tokens)**
- Configured in `Program.cs`
- Validates:
  - Issuer
  - Audience
  - Expiration time
  - Private key (from environment variables)

### Authorization
- **`[Authorize]` attribute** on protected controllers/actions
- Some endpoints are public (register, login, list courses)
- Sensitive operations require JWT token

### Environment Variables
Loaded from `.env`:
- `JWTSETTINGS__SECRET` - Key for signing JWT
- `JWTSETTINGS__ISSUER` - Token issuer
- `JWTSETTINGS__AUDIENCE` - Token audience
- `DB_HOST`, `DB_PORT`, `DB_NAME`, `DB_USER`, `DB_PASSWORD` - Database connection

---

## 📦 Main Dependencies

| Package | Version | Purpose |
|---------|---------|----------|
| **AutoMapper** | 12.0.1 | Automatic mapping between entities and DTOs |
| **AutoMapper.Extensions.Microsoft.DependencyInjection** | 12.0.1 | AutoMapper integration with DI |
| **Entity Framework Core** | (Net8.0) | ORM for data access |
| **Pomelo.EntityFrameworkCore.MySql** | (Net8.0) | MySQL driver for EF |
| **Microsoft.AspNetCore.Identity** | 2.3.1 | User authentication and management |
| **System.IdentityModel.Tokens.Jwt** | 8.0.2 | JWT validation |
| **Microsoft.Extensions.Configuration.Abstractions** | 10.0.1 | Configuration |
| **DotNetEnv** | (Net8.0) | Load variables from `.env` |
| **Swashbuckle.AspNetCore** | (Net8.0) | Swagger/OpenAPI |

---

## 🗄️ Database

### Technology
- **Engine**: MySQL 8.0+
- **ORM**: Entity Framework Core

### Database Contexts

#### AppDbContext
Business entities:
- `Courses` - Courses table
- `Lessons` - Lessons table

#### AuthDbContext
Identity management:
- `AspNetUsers` - Users
- `AspNetRoles` - Roles
- Other Identity tables (claims, logins, etc.)

### Main Entities

**Course**
- `Id` (Guid)
- `Title` (string)
- `Status` (CourseStatus: Draft, Published, Archived)
- `IsDeleted` (bool) - Soft delete
- `CreatedAt`, `UpdatedAt` (DateTime)
- `Lessons` (ICollection) - 1:N relationship

**Lesson**
- `Id` (Guid)
- `CourseId` (Guid) - FK
- `Title` (string)
- `Order` (int)
- `IsDeleted` (bool) - Soft delete
- `CreatedAt`, `UpdatedAt` (DateTime)

**ApplicationUser**
- Extends `IdentityUser`
- Standard user properties (Email, UserName, etc.)

---

## 🎯 Patterns and Principles

### Design Patterns Used
- **Repository Pattern** - Data access abstraction
- **Clean Architecture** - Separation of concerns across layers
- **DTO Pattern** - Safe data transfer
- **CQRS (prepared)** - Command and Query separation

### SOLID Principles
- **S** (Single Responsibility) - Each class has one responsibility
- **O** (Open/Closed) - Open for extension, closed for modification
- **L** (Liskov Substitution) - Interfaces for dependency injection
- **I** (Interface Segregation) - Specific interfaces
- **D** (Dependency Inversion) - Dependency injection

### Soft Delete
- Entities are not physically deleted
- Marked with `IsDeleted = true`
- Queries automatically filter deleted records

---

## 🚀 Project Execution

### Requirements
- .NET 8 SDK
- MySQL 8.0+
- Environment variables in `.env`

### Steps
1. Restore packages: `dotnet restore`
2. Apply migrations: `dotnet ef database update`
3. Run: `dotnet run --project Presentation`
4. Access Swagger: `http://localhost:5129`

---

## 📝 Additional Notes

- **CORS enabled** for all origins (consider restricting in production)
- **Swagger available** in development for interactive documentation
- **Logging** prepared with standard .NET infrastructure
- **Business validations** in entities and services
- **Exception handling** with appropriate HTTP status codes

---

**Last Updated:** January 2026

