# Assessment - Course Management System

A modern course management API built with **ASP.NET Core 8**, following **Clean Architecture** principles with JWT authentication, MySQL database, and comprehensive API documentation.

---



## Overview

This project is a comprehensive course management system that allows users to:
- ✅ Create and manage courses
- ✅ Add lessons to courses with ordering
- ✅ Publish/unpublish courses
- ✅ User authentication with JWT tokens
- ✅ Role-based access control
- ✅ Complete REST API with Swagger documentation

**Key Features:**
- Clean Architecture (Domain, Application, Infrastructure, Presentation layers)
- Repository Pattern for data access
- AutoMapper for DTO mapping
- JWT-based authentication and authorization
- Soft delete pattern for data consistency
- MySQL database with Entity Framework Core
- Comprehensive unit tests for business rules

---

## Technology Stack

### Backend
| Technology | Version | Purpose |
|------------|---------|---------|
| **ASP.NET Core** | 8.0     | Web framework |
| **C#** | 12      | Programming language |
| **Entity Framework Core** | 8.0     | ORM |
| **MySQL** | 8.0+    | Database |
| **AutoMapper** | 12.0.1  | DTO mapping |
| **JWT (System.IdentityModel.Tokens.Jwt)** | 8.0.2   | Authentication |
| **Swagger/Swashbuckle** | Latest  | API documentation |
| **xUnit** | 2.5.3   | Unit testing framework |

### Development Tools
- Visual Studio 2022 or JetBrains Rider
- .NET 8 SDK
- MySQL Server 8.0+
- Git

---

## Prerequisites

Before you begin, ensure you have the following installed:

1. **.NET 8 SDK** or later
   ```bash
   dotnet --version  # Check your version
   ```
   [Download .NET 8](https://dotnet.microsoft.com/download/dotnet/8.0)

2. **MySQL Server 8.0** or later
   ```bash
   mysql --version  # Check your version
   ```
   [Download MySQL](https://dev.mysql.com/downloads/mysql/)

3. **Git**
   ```bash
   git --version
   ```
   [Download Git](https://git-scm.com/)

4. **IDE** (choose one):
   - Visual Studio 2022 Community (free)
   - JetBrains Rider
   - Visual Studio Code

---

## Installation & Setup

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/assessment.git
cd assessment
```

### 2. Install .NET Dependencies

```bash
dotnet restore
```

This will restore all NuGet packages defined in the `.csproj` files.

### 3. Verify Project Structure

```bash
tree -L 2
```

Expected structure:
```
Assessment/
├── Domain/                  # Business logic & entities
├── Application/             # Use cases & DTOs
├── Infrastructure/          # Data access & repositories
├── Presentation/            # API controllers
├── TestRuleOfNegocition/    # Unit tests
├── API.md                   # API documentation
├── ARCHITECTURE.md          # Architecture documentation
└── README.md               # This file
```

---

## Configuration

### 1. Environment Variables Setup

Create or update the `.env` file in the project root:

```bash
# Copy the example if it doesn't exist
cp .env.example .env  # (if available) or create manually
```

**`.env` File Content:**

```dotenv
# Database Configuration
DB_HOST=localhost
DB_PORT=3306
DB_NAME=assessment_db
DB_USER=root
DB_PASSWORD=your_mysql_password

# JWT Configuration
JWTSETTINGS__SECRET=YourSuperSecretKeyForJWTTokenGenerationThatIsAtLeast32CharactersLong
JWTSETTINGS__ISSUER=CoursesAPI
JWTSETTINGS__AUDIENCE=CoursesAPIUsers
JWTSETTINGS__EXPIRATIONMINUTES=60
```

**Important:** 
- Change `DB_PASSWORD` to your actual MySQL root password
- `DB_HOST` defaults to `localhost` for local development
- `JWTSETTINGS__SECRET` should be at least 32 characters (change for production)

### 2. Update appsettings.json (if needed)

The `appsettings.json` files are already configured to read from environment variables:

```bash
cat Presentation/appsettings.Development.json
```

---

## Database Setup

### 1. Create MySQL Database

**Option A: Using MySQL Command Line**

```bash
# Connect to MySQL
mysql -u root -p

# Create database
CREATE DATABASE assessment_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

# Verify creation
SHOW DATABASES;

# Exit
EXIT;
```

**Option B: Using MySQL Workbench**

1. Open MySQL Workbench
2. Create new connection
3. Right-click "Databases" → "Create New Database"
4. Name: `assessment_db`
5. Click "Apply"

### 2. Apply Entity Framework Migrations

```bash
# Navigate to Infrastructure project directory
cd Infrastructure

# Add and apply migrations
dotnet ef database update

# Or apply migrations from project root
dotnet ef database update --project Infrastructure
```

**Expected Output:**
```
Build started...
Build completed successfully.
Applying migration '20260105195401_Version initial of database'.
Applying migration '20260105195722_Initial Version of database'.
Applying migration '20260105201905_New table User'.
Done.
```

### 3. Verify Database Tables

```bash
# Connect to MySQL
mysql -u root -p assessment_db

# List all tables
SHOW TABLES;

# Verify structure
DESCRIBE Courses;
DESCRIBE Lessons;
DESCRIBE AspNetUsers;

# Exit
EXIT;
```

**Expected Tables:**
- `Courses` - Course entities
- `Lessons` - Lesson entities
- `AspNetUsers` - User accounts
- `AspNetRoles` - Role definitions
- `AspNetUserRoles` - User-role mappings
- Other Identity tables

---

## Running the Application

### 1. From Command Line

**Option A: Run from Project Root**

```bash
# Restore and build
dotnet restore
dotnet build

# Run the API
dotnet run --project Presentation
```

**Option B: Run from IDE**

1. Open the solution in Visual Studio or Rider
2. Set `Presentation` as startup project
3. Press `F5` or click "Run"

### 2. Verify API is Running

The API will start on:
```
http://localhost:5129
```

**Check Health:**

```bash
# Open browser or use curl
curl http://localhost:5129

# Or access Swagger UI
curl http://localhost:5129/swagger/index.html
```

### 3. Access Swagger Documentation

Open your browser and navigate to:

```
http://localhost:5129/swagger/index.html
```

You will see the interactive API documentation where you can:
- View all available endpoints
- Try out API calls
- Test authentication with JWT tokens

---

## API Documentation

Complete API documentation is available in the repository:

- **`API.md`** - Detailed endpoint documentation
  - All available routes
  - Request/response examples
  - Authentication requirements
  - Error codes

- **`ARCHITECTURE.md`** - System architecture details
  - Layered architecture explanation
  - Design patterns used
  - Database schema
  - Security implementation

### Quick API Examples

**Register a New User:**

```bash
curl -X POST http://localhost:5129/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "student@example.com",
    "password": "Password123",
    "userName": "student001"
  }'
```

**Login and Get JWT Token:**

```bash
curl -X POST http://localhost:5129/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "student@example.com",
    "password": "Password123"
  }'

# Response will contain:
# {
#   "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
#   "user": { ... }
# }
```

**Create a Course:**

```bash
curl -X POST http://localhost:5129/api/v1/course \
  -H "Content-Type: application/json" \
  -d '"Advanced C# Programming"'
```

**List All Courses:**

```bash
curl http://localhost:5129/api/v1/course?page=1&pageSize=10
```

For more examples, refer to `API.md` in the project root.

---

## Test Credentials

### Pre-configured Test User (if database seeding is implemented)

**Note:** Currently, the system requires manual user creation. Use the registration endpoint to create test users.

### Creating Test Users

Use the API registration endpoint:

```bash
# User 1: Teacher
curl -X POST http://localhost:5129/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "teacher@assessment.local",
    "password": "Teacher@123",
    "userName": "teacher_001"
  }'

# User 2: Student
curl -X POST http://localhost:5129/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "student@assessment.local",
    "password": "Student@123",
    "userName": "student_001"
  }'
```

### Using Test Credentials

1. Login to get JWT token:
```bash
curl -X POST http://localhost:5129/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "teacher@assessment.local",
    "password": "Teacher@123"
  }'
```

2. Save the returned token

3. Use token in protected endpoints:
```bash
curl -X GET http://localhost:5129/api/v1/course/search \
  -H "Authorization: Bearer <YOUR_TOKEN_HERE>"
```

---

## Project Structure

```
Assessment/
│
├── Domain/                      # Layer 1: Pure business logic
│   ├── Entities/               # Course, Lesson, ApplicationUser
│   ├── Enums/                  # CourseStatus
│   └── Interfaces/             # Repository contracts
│
├── Application/                 # Layer 2: Use cases & orchestration
│   ├── DTOs/                   # Data Transfer Objects
│   ├── Interfaces/             # Service contracts
│   ├── Services/               # Business logic implementation
│   ├── Mappings/               # AutoMapper profiles
│   └── UseCases/               # Command/Query handlers
│
├── Infrastructure/              # Layer 3: External concerns
│   ├── Persistence/            # Database contexts
│   ├── Repositories/           # Repository implementations
│   ├── Migrations/             # EF Core migrations
│   └── Services/               # Infrastructure services
│
├── Presentation/                # Layer 4: REST API
│   ├── Controllers/            # API endpoints
│   ├── Program.cs              # Startup configuration
│   └── appsettings.json        # Configuration files
│
├── TestRuleOfNegocition/        # Unit tests
│   └── UnitTest1.cs            # Business rule tests
│
├── API.md                       # API documentation
├── ARCHITECTURE.md              # Architecture documentation
├── README.md                    # This file
└── .env                         # Environment variables
```

---

## Development

### Running Unit Tests

```bash
# Run all tests
dotnet test

# Run tests with verbose output
dotnet test -v detailed

# Run specific test project
dotnet test TestRuleOfNegocition/TestRuleOfNegocition.csproj

# Run with code coverage
dotnet test /p:CollectCoverage=true
```

**Test Results:**

The project includes 11 unit tests validating critical business rules:

```
✅ PublishCourse_WithLessons_ShouldSucceed
✅ PublishCourse_WithoutLessons_ShouldFail
✅ CreateLesson_WithUniqueOrder_ShouldSucceed
✅ CreateLesson_WithDuplicateOrder_ShouldFail
✅ DeleteCourse_ShouldBeSoftDelete
✅ PublishCourse_WithOnlyDeletedLessons_ShouldFail
✅ CreateMultipleLessons_WithSequentialOrders_ShouldSucceed
✅ CreateCourse_ShouldHaveDraftStatus
✅ UnpublishCourse_FromPublished_ShouldRevertToDraft
✅ ReorderLesson_ToDuplicateOrder_ShouldFail
✅ ReorderLesson_ToValidOrder_ShouldSucceed
```

### Building the Solution

```bash
# Clean previous builds
dotnet clean

# Restore dependencies
dotnet restore

# Build all projects
dotnet build

# Build in Release mode
dotnet build -c Release
```

### Code Style & Formatting

The project follows C# coding standards. Use your IDE's formatting tools:

- **Visual Studio:** `Ctrl+K, Ctrl+D`
- **Rider:** `Ctrl+Alt+L`

---

## API Endpoints Summary

### Authentication
- `POST /api/v1/auth/register` - Register new user
- `POST /api/v1/auth/login` - Login and get JWT token

### Courses
- `GET /api/v1/course` - List all courses
- `GET /api/v1/course/{id}` - Get course details
- `GET /api/v1/course/search` - Search courses (requires auth)
- `GET /api/v1/course/{id}/summary` - Get course summary (requires auth)
- `POST /api/v1/course` - Create new course
- `POST /api/v1/course/{id}/publish` - Publish course (requires auth)
- `POST /api/v1/course/{id}/unpublish` - Unpublish course (requires auth)
- `DELETE /api/v1/course/{id}` - Delete course

### Lessons
- `GET /api/v1/course/{courseId}/lesson` - List lessons
- `POST /api/v1/course/{courseId}/lesson` - Add lesson
- `PUT /api/v1/course/{courseId}/lesson/{lessonId}/reorder` - Reorder lesson
- `DELETE /api/v1/course/{courseId}/lesson/{lessonId}` - Delete lesson

For detailed documentation, see `API.md`.

---

## Troubleshooting

### Common Issues

#### 1. Database Connection Error

**Error:** `Failed to connect to MySQL server`

**Solution:**

```bash
# Check MySQL is running
mysql --version

# On Windows (if installed)
net start MySQL80

# On macOS
brew services start mysql

# Verify connection
mysql -u root -p -h localhost

# Check .env file has correct credentials
cat .env
```

#### 2. Migration Errors

**Error:** `No migrations found in 'Infrastructure'`

**Solution:**

```bash
# Ensure you're in the right directory
cd Infrastructure

# List existing migrations
dotnet ef migrations list

# Apply migrations
dotnet ef database update

# If migrations are missing, create them
dotnet ef migrations add InitialMigration
```

#### 3. Port Already in Use

**Error:** `Address 127.0.0.1:5129 already in use`

**Solution:**

```bash
# Kill process on port 5129
# On Linux/macOS
lsof -ti:5129 | xargs kill -9

# On Windows
netstat -ano | findstr :5129
taskkill /PID <PID> /F

# Or change port in launchSettings.json
```

#### 4. JWT Token Issues

**Error:** `Unauthorized - Invalid token`

**Solution:**

```bash
# Verify JWT secret in .env
cat .env | grep JWTSETTINGS__SECRET

# Ensure token is included in header
Authorization: Bearer <token>

# Token format should be: "Bearer " + token_value
```

#### 5. AutoMapper Configuration Error

**Error:** `System.MissingMethodException: Method not found`

**Solution:**

```bash
# This was fixed in the initial setup
# If you encounter this, verify AutoMapper versions match:
dotnet list package | grep AutoMapper

# Should show both:
# - AutoMapper 12.0.1
# - AutoMapper.Extensions.Microsoft.DependencyInjection 12.0.1
```

### Debug Mode

Run the application in debug mode for detailed error information:

```bash
# Run with debug output
dotnet run --project Presentation --configuration Debug

# Enable detailed logging
# Edit appsettings.Development.json and set:
# "Logging": {
#   "LogLevel": {
#     "Default": "Debug",
#     "Microsoft": "Information"
#   }
# }
```

---

## Performance Optimization

### Database Optimization

```bash
# Add indexes (manual SQL if needed)
# The migrations already include necessary indexes

# Monitor slow queries
# Enable MySQL slow query log (optional)
```

### API Performance

- Pagination is implemented (default: 10 items per page)
- Soft delete prevents full deletes (better performance)
- AutoMapper caches mappings
- Repository pattern enables caching strategies

---

## Production Deployment

### Before Deploying to Production

1. **Update JWT Secret**
   ```
   JWTSETTINGS__SECRET=<your-production-secret-key>
   ```

2. **Update Database Connection**
   ```
   DB_HOST=<production-db-host>
   DB_NAME=<production-db-name>
   DB_USER=<production-db-user>
   DB_PASSWORD=<secure-password>
   ```

3. **Restrict CORS**
   - Update `Program.cs` CORS policy
   - Only allow trusted origins

4. **Build Release Version**
   ```bash
   dotnet build -c Release
   ```

5. **Run Tests**
   ```bash
   dotnet test -c Release
   ```

---

## Additional Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [JWT Authentication](https://tools.ietf.org/html/rfc7519)
- [MySQL Documentation](https://dev.mysql.com/doc/)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

---

## Support & Contribution

For issues, questions, or contributions:

1. Check existing documentation (`API.md`, `ARCHITECTURE.md`)
2. Review unit tests for usage examples
3. Check troubleshooting section above
4. Create an issue with detailed information

---

## License

This project is provided as-is for educational and assessment purposes.

---

**Last Updated:** January 2026
**ASP.NET Core Version:** 8.0
**Database:** MySQL 8.0+

