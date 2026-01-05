# API Documentation - Routes and Protection

## 🔗 Base URL
```
http://localhost:5129/api/v1
```

---

## 🔐 Authentication

### How to Use JWT

1. **Register or Login** to obtain a token
2. **Include the token** in the `Authorization` header:
   ```
   Authorization: Bearer <your_jwt_token>
   ```

### Example with cURL
```bash
curl -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIs..." \
     http://localhost:5129/api/v1/course/search
```

---

## 📚 Endpoints by Module

---

# 🔑 Auth - Authentication

## `POST /auth/register`
**Description:** Register a new user in the system

**Protection:** ❌ Public (no authentication required)

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "myPassword123",
  "userName": "user123"
}
```

**Response (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "uuid",
    "email": "user@example.com",
    "userName": "user123"
  }
}
```

**Possible Errors:**
- `400 Bad Request` - Email or username already exists, invalid password
- `400 Bad Request` - Incomplete or invalid data

---

## `POST /auth/login`
**Description:** Authenticate a user and obtain a JWT token

**Protection:** ❌ Public (no authentication required)

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "myPassword123"
}
```

**Response (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "uuid",
    "email": "user@example.com",
    "userName": "user123"
  }
}
```

**Possible Errors:**
- `401 Unauthorized` - Invalid credentials
- `400 Bad Request` - Incomplete data

---

# 📖 Courses - Course Management

## `GET /course`
**Description:** Get a paginated list of all courses

**Protection:** ❌ Public (no authentication required)

**Query Parameters:**
- `page` (int, default: 1) - Page number
- `pageSize` (int, default: 10) - Number of records per page

**Request Example:**
```
GET /course?page=1&pageSize=10
```

**Response (200 OK):**
```json
{
  "items": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "title": "Advanced C#",
      "status": "Published",
      "createdAt": "2026-01-05T10:00:00",
      "updatedAt": "2026-01-05T12:00:00"
    }
  ],
  "totalCount": 25,
  "page": 1,
  "pageSize": 10
}
```

---

## `GET /course/search`
**Description:** Search courses with filters (requires authentication)

**Protection:** 🔒 Requires JWT

**Query Parameters:**
- `q` (string, optional) - Search term in title
- `status` (string, optional) - Filter by status (Draft, Published, Archived)
- `page` (int, default: 1) - Page number
- `pageSize` (int, default: 10) - Number of records per page

**Request Example:**
```
GET /course/search?q=python&status=Published&page=1&pageSize=10
Authorization: Bearer <token>
```

**Response (200 OK):**
```json
{
  "items": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "title": "Advanced Python",
      "status": "Published",
      "createdAt": "2026-01-01T10:00:00",
      "updatedAt": "2026-01-05T12:00:00"
    }
  ],
  "totalCount": 5,
  "page": 1,
  "pageSize": 10
}
```

**Possible Errors:**
- `401 Unauthorized` - Token not provided or invalid
- `403 Forbidden` - Token expired

---

## `GET /course/{id}`
**Description:** Get full details of a course

**Protection:** ❌ Public (no authentication required)

**Path Parameters:**
- `id` (Guid) - Course ID

**Request Example:**
```
GET /course/550e8400-e29b-41d4-a716-446655440000
```

**Response (200 OK):**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "title": "Advanced C#",
  "status": "Published",
  "createdAt": "2026-01-05T10:00:00",
  "updatedAt": "2026-01-05T12:00:00"
}
```

**Possible Errors:**
- `404 Not Found` - Course does not exist or was deleted

---

## `GET /course/{id}/summary`
**Description:** Get a course summary with statistics

**Protection:** 🔒 Requires JWT

**Path Parameters:**
- `id` (Guid) - Course ID

**Request Example:**
```
GET /course/550e8400-e29b-41d4-a716-446655440000/summary
Authorization: Bearer <token>
```

**Response (200 OK):**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "title": "Advanced C#",
  "totalLessons": 15,
  "lastModified": "2026-01-05T12:00:00"
}
```

**Possible Errors:**
- `401 Unauthorized` - Token not provided or invalid
- `404 Not Found` - Course does not exist

---

## `POST /course`
**Description:** Create a new course in Draft status

**Protection:** ❌ Public (no authentication required)

**Request Body:**
```json
"My New JavaScript Course"
```

or

```json
{
  "title": "My New JavaScript Course"
}
```

**Response (201 Created):**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440010"
}
```

**Response Headers:**
```
Location: /api/v1/course/550e8400-e29b-41d4-a716-446655440010/summary
```

---

## `POST /course/{id}/publish`
**Description:** Publish a course (requires at least one active lesson)

**Protection:** 🔒 Requires JWT

**Path Parameters:**
- `id` (Guid) - Course ID

**Request Example:**
```
POST /course/550e8400-e29b-41d4-a716-446655440000/publish
Authorization: Bearer <token>
```

**Response (200 OK):**
```json
{}
```

**Possible Errors:**
- `401 Unauthorized` - Token not provided or invalid
- `404 Not Found` - Course does not exist
- `400 Bad Request` - Course has no active lessons

---

## `POST /course/{id}/unpublish`
**Description:** Unpublish a course (revert to Draft status)

**Protection:** 🔒 Requires JWT

**Path Parameters:**
- `id` (Guid) - Course ID

**Request Example:**
```
POST /course/550e8400-e29b-41d4-a716-446655440000/unpublish
Authorization: Bearer <token>
```

**Response (200 OK):**
```json
{
  "message": "Course unpublished successfully"
}
```

**Possible Errors:**
- `401 Unauthorized` - Token not provided or invalid
- `404 Not Found` - Course does not exist

---

## `DELETE /course/{id}`
**Description:** Delete a course (soft delete)

**Protection:** ❌ Public (no authentication required)

**Path Parameters:**
- `id` (Guid) - Course ID

**Request Example:**
```
DELETE /course/550e8400-e29b-41d4-a716-446655440000
```

**Response (204 No Content):**
```
(no body)
```

**Possible Errors:**
- `404 Not Found` - Course does not exist or already deleted

---

# 📝 Lessons - Lesson Management

## `GET /course/{courseId}/lesson`
**Description:** List lessons of a course with pagination

**Protection:** ❌ Public (no authentication required)

**Path Parameters:**
- `courseId` (Guid) - Course ID

**Query Parameters:**
- `page` (int, default: 1) - Page number
- `pageSize` (int, default: 10) - Number of records per page

**Request Example:**
```
GET /course/550e8400-e29b-41d4-a716-446655440000/lesson?page=1&pageSize=10
```

**Response (200 OK):**
```json
{
  "items": [
    {
      "id": "660e8400-e29b-41d4-a716-446655440001",
      "title": "Introduction to Async/Await",
      "order": 1,
      "createdAt": "2026-01-05T10:00:00",
      "updatedAt": "2026-01-05T10:00:00"
    },
    {
      "id": "660e8400-e29b-41d4-a716-446655440002",
      "title": "Advanced Patterns",
      "order": 2,
      "createdAt": "2026-01-05T10:00:00",
      "updatedAt": "2026-01-05T10:00:00"
    }
  ],
  "totalCount": 15,
  "page": 1,
  "pageSize": 10
}
```

**Possible Errors:**
- `404 Not Found` - Course does not exist

---

## `POST /course/{courseId}/lesson`
**Description:** Add a new lesson to a course

**Protection:** ❌ Public (no authentication required)

**Path Parameters:**
- `courseId` (Guid) - Course ID

**Request Body:**
```json
{
  "title": "New Features in C# 12",
  "order": 3
}
```

**Response (201 Created):**
```json
{
  "id": "660e8400-e29b-41d4-a716-446655440003"
}
```

**Response Headers:**
```
Location: /api/v1/course/550e8400-e29b-41d4-a716-446655440000/lesson
```

**Possible Errors:**
- `404 Not Found` - Course does not exist
- `400 Bad Request` - Invalid data (duplicate order, etc.)

---

## `PUT /course/{courseId}/lesson/{lessonId}/reorder`
**Description:** Change the order of a lesson within a course

**Protection:** ❌ Public (no authentication required)

**Path Parameters:**
- `courseId` (Guid) - Course ID
- `lessonId` (Guid) - Lesson ID

**Request Body:**
```json
5
```

**Response (204 No Content):**
```
(no body)
```

**Possible Errors:**
- `404 Not Found` - Course or lesson does not exist
- `400 Bad Request` - Invalid or duplicate order

---

## `DELETE /course/{courseId}/lesson/{lessonId}`
**Description:** Delete a lesson from a course (soft delete)

**Protection:** ❌ Public (no authentication required)

**Path Parameters:**
- `courseId` (Guid) - Course ID
- `lessonId` (Guid) - Lesson ID

**Request Example:**
```
DELETE /course/550e8400-e29b-41d4-a716-446655440000/lesson/660e8400-e29b-41d4-a716-446655440001
```

**Response (204 No Content):**
```
(no body)
```

**Possible Errors:**
- `404 Not Found` - Course or lesson does not exist or already deleted

---

## 📊 Protection Summary by Endpoint

### 🔒 Require JWT (Authentication)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/course/search` | Search courses |
| GET | `/course/{id}/summary` | Course summary |
| POST | `/course/{id}/publish` | Publish course |
| POST | `/course/{id}/unpublish` | Unpublish course |

### ❌ Public (No authentication required)
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/auth/register` | Register user |
| POST | `/auth/login` | Login |
| GET | `/course` | List courses |
| GET | `/course/{id}` | Get course |
| POST | `/course` | Create course |
| DELETE | `/course/{id}` | Delete course |
| GET | `/course/{courseId}/lesson` | List lessons |
| POST | `/course/{courseId}/lesson` | Add lesson |
| PUT | `/course/{courseId}/lesson/{lessonId}/reorder` | Reorder lesson |
| DELETE | `/course/{courseId}/lesson/{lessonId}` | Delete lesson |

---

## 🧪 Complete Flow Example

### 1. Register user
```bash
curl -X POST http://localhost:5129/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "teacher@example.com",
    "password": "Password123",
    "userName": "teacher001"
  }'
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": { "id": "abc123...", "email": "teacher@example.com" }
}
```

### 2. Save the token
```bash
TOKEN="eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

### 3. Create a course
```bash
curl -X POST http://localhost:5129/api/v1/course \
  -H "Content-Type: application/json" \
  -d '"My Advanced C# Course"'
```

**Response:**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000"
}
```

### 4. Add lessons to the course
```bash
curl -X POST http://localhost:5129/api/v1/course/550e8400-e29b-41d4-a716-446655440000/lesson \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Async/Await",
    "order": 1
  }'
```

### 5. Publish the course (requires authentication)
```bash
curl -X POST http://localhost:5129/api/v1/course/550e8400-e29b-41d4-a716-446655440000/publish \
  -H "Authorization: Bearer $TOKEN"
```

### 6. Search published courses (requires authentication)
```bash
curl -X GET "http://localhost:5129/api/v1/course/search?status=Published" \
  -H "Authorization: Bearer $TOKEN"
```

---

## 📋 HTTP Status Codes

| Code | Meaning | Use Case |
|--------|---------|----------|
| **200** | OK | Successful request |
| **201** | Created | Resource created successfully |
| **204** | No Content | Successful operation with no response |
| **400** | Bad Request | Invalid or malformed data |
| **401** | Unauthorized | Token not provided or invalid |
| **403** | Forbidden | Token expired or no permissions |
| **404** | Not Found | Resource does not exist |
| **500** | Internal Server Error | Server error |

---

## 🔐 JWT Information

### Token Structure
```
header.payload.signature
```

### Typical Payload Content
```json
{
  "sub": "user_id",
  "email": "user@example.com",
  "iat": 1234567890,
  "exp": 1234571490,
  "iss": "issuer",
  "aud": "audience"
}
```

### Server-Side Validation
- ✅ Valid signature
- ✅ Not expired
- ✅ Issuer matches
- ✅ Audience matches

---

## 📌 Important Notes

1. **Soft Delete:** Deleted courses and lessons are not physically removed, only marked as `IsDeleted: true`
2. **Pagination:** All endpoints returning lists use pagination (default: page 1, 10 items)
3. **CORS:** Enabled for all origins (consider restricting in production)
4. **Swagger:** Interactive documentation available at `http://localhost:5129` (development)
5. **API Versioning:** Endpoints under `/api/v1` for future compatibility

---

**Last Updated:** January 2026
**API Version:** v1

