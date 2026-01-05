# API Routes - Assessment

**Base URL:** `http://localhost:5129/api/v1`

---

## 🔐 Authentication

All protected endpoints require JWT token in header:
```
Authorization: Bearer <your_jwt_token>
```

---

## 📋 All Routes

### Auth - `POST /auth/*`

| Method | Endpoint | Protected | Description |
|--------|----------|:---------:|-------------|
| POST | `/auth/register` | ❌ | Register new user |
| POST | `/auth/login` | ❌ | Login & get JWT token |

### Courses - `GET/POST /courses/*`

| Method | Endpoint | Protected | Description |
|--------|----------|:---------:|-------------|
| GET | `/courses` | ❌ | List all courses (paginated) |
| GET | `/courses/search` | 🔒 | Search courses with filters |
| GET | `/courses/{id}` | ❌ | Get course details |
| GET | `/courses/{id}/summary` | 🔒 | Get course summary with stats |
| POST | `/courses` | ❌ | Create new course |
| POST | `/courses/{id}/publish` | 🔒 | Publish course |
| POST | `/courses/{id}/unpublish` | 🔒 | Unpublish course |
| DELETE | `/courses/{id}` | ❌ | Delete course (soft delete) |

### Lessons - `GET/POST /courses/{courseId}/lessons/*`

| Method | Endpoint | Protected | Description |
|--------|----------|:---------:|-------------|
| GET | `/courses/{courseId}/lessons` | ❌ | List course lessons (paginated) |
| GET | `/courses/{courseId}/lessons/{id}` | ❌ | Get specific lesson |
| POST | `/courses/{courseId}/lessons` | ❌ | Add lesson to course |
| PUT | `/courses/{courseId}/lessons/{lessonId}/reorder` | ❌ | Change lesson order |
| DELETE | `/courses/{courseId}/lessons/{lessonId}` | ❌ | Delete lesson (soft delete) |

---

## ✅ Summary

- **Total Routes:** 14
- **Protected (JWT Required):** 4
  - GET `/courses/search`
  - GET `/courses/{id}/summary`
  - POST `/courses/{id}/publish`
  - POST `/courses/{id}/unpublish`
- **Public:** 10

**Legend:**
- ❌ = Public (no authentication)
- 🔒 = Protected (requires JWT token)

**Last Updated:** January 2026


