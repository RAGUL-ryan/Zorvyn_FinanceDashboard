# Finance Dashboard API

A production-grade **.NET 8 REST API** for managing financial records with JWT authentication,
role-based access control, EF Core + MySQL, Swagger UI, rate limiting, soft delete, pagination,
search, and unit tests.

---

## Architecture — 3-Tier

```
FinanceDashboard/
├── FinanceDashboard.API/          ← Presentation Layer  (Controllers, Middleware, Filters)
├── FinanceDashboard.Business/     ← Business Logic Layer (Services, DTOs, Validators)
├── FinanceDashboard.Data/         ← Data Access Layer    (EF Core, Repositories, Entities)
└── FinanceDashboard.Tests/        ← Unit Tests           (xUnit, Moq, FluentAssertions)
```

## Roles & Permissions

| Endpoint                      | Admin | Analyst | Viewer |
|-------------------------------|:-----:|:-------:|:------:|
| Login / Refresh / Logout      | ✅    | ✅      | ✅     |
| GET /transactions             | ✅    | ✅      | ✅     |
| POST/PUT /transactions        | ✅    | ✅      | ❌     |
| DELETE /transactions          | ✅    | ❌      | ❌     |
| GET /dashboard/*              | ✅    | ✅      | ✅     |
| GET/POST/PUT/DELETE /users    | ✅    | ❌      | ❌     |
| PATCH /users/{id}/status      | ✅    | ❌      | ❌     |

---

## Prerequisites

| Tool         | Version   | Download |
|--------------|-----------|----------|
| .NET SDK     | 8.0+      | https://dotnet.microsoft.com/download |
| MySQL Server | 8.0+      | https://dev.mysql.com/downloads/mysql/ |
| Git          | any       | https://git-scm.com |

---

## Step-by-Step Setup

### STEP 1 — Clone / extract the project

```bash
# If using git
git clone <your-repo-url>
cd FinanceDashboard

# Or extract the zip and cd into the folder
cd FinanceDashboard
```

### STEP 2 — Set up MySQL

Start MySQL and run:

```sql
-- Option A: Let EF Core create everything automatically (recommended)
CREATE DATABASE FinanceDashboard CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```

Or run the full manual script (creates tables + seed data):

```bash
mysql -u root -p < schema.sql
```

### STEP 3 — Configure the connection string

Edit `FinanceDashboard.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=FinanceDashboard;User=root;Password=YOUR_MYSQL_PASSWORD;"
  },
  "Jwt": {
    "Key": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "FinanceDashboardAPI",
    "Audience": "FinanceDashboardClient",
    "ExpiryMinutes": "60"
  }
}
```

> **Important:** Replace `YOUR_MYSQL_PASSWORD` with your actual MySQL root password.  
> The JWT Key must be at least 32 characters.

### STEP 4 — Restore NuGet packages

```bash
dotnet restore
```

### STEP 5 — Apply EF Core migrations (creates tables + seeds data automatically)

```bash
# Install EF Core tools (only needed once)
dotnet tool install --global dotnet-ef

# Run migrations (auto-applied on startup too, but can do it manually)
dotnet ef database update --project FinanceDashboard.Data --startup-project FinanceDashboard.API
```

> ℹ️ The app also calls `db.Database.Migrate()` on startup, so migrations apply automatically.

### STEP 6 — Build the solution

```bash
dotnet build
```

### STEP 7 — Run the API

```bash
cd FinanceDashboard.API
dotnet run
```

You should see output like:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
```

### STEP 8 — Open Swagger UI

Navigate to: **http://localhost:5000**

Swagger UI loads at the root URL with full interactive API documentation.

### STEP 9 — Run the tests

```bash
cd ..   # back to solution root
dotnet test --verbosity normal
```

---

## API Quick Start

### 1. Login as Admin

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "admin@finance.com",
  "password": "Admin@123"
}
```

Response:
```json
{
  "success": true,
  "data": {
    "accessToken": "eyJhbGc...",
    "refreshToken": "base64string...",
    "expiresAt": "2025-01-01T01:00:00Z",
    "user": { "id": 1, "fullName": "System Admin", "email": "admin@finance.com", "role": "Admin" }
  }
}
```

### 2. Authorize in Swagger

Click **Authorize** → paste: `Bearer eyJhbGc...`

### 3. Create a transaction

```http
POST /api/transactions
Authorization: Bearer <token>
Content-Type: application/json

{
  "amount": 5000.00,
  "type": "Income",
  "category": "Salary",
  "date": "2025-01-15T00:00:00Z",
  "description": "Monthly salary",
  "notes": "January paycheck"
}
```

### 4. Get dashboard summary

```http
GET /api/dashboard/summary?from=2025-01-01&to=2025-01-31
Authorization: Bearer <token>
```

### 5. List transactions with filters, search & pagination

```http
GET /api/transactions?type=Expense&category=Food&page=1&pageSize=10&search=grocery
Authorization: Bearer <token>
```

### 6. Refresh token

```http
POST /api/auth/refresh-token
Content-Type: application/json

{ "refreshToken": "base64string..." }
```

---

## All Endpoints

### Auth
| Method | Endpoint                    | Auth | Description            |
|--------|-----------------------------|------|------------------------|
| POST   | /api/auth/login             | No   | Login, get JWT tokens  |
| POST   | /api/auth/refresh-token     | No   | Refresh access token   |
| POST   | /api/auth/logout            | Yes  | Logout, clear token    |
| GET    | /api/auth/me                | Yes  | Get current user info  |

### Users (Admin only)
| Method | Endpoint                      | Description           |
|--------|-------------------------------|-----------------------|
| GET    | /api/users                    | List all users        |
| GET    | /api/users/{id}               | Get user by ID        |
| POST   | /api/users                    | Create user           |
| PUT    | /api/users/{id}               | Update user           |
| DELETE | /api/users/{id}               | Soft-delete user      |
| PATCH  | /api/users/{id}/status        | Activate/deactivate   |
| POST   | /api/users/change-password    | Change own password   |

### Transactions
| Method | Endpoint                     | Roles           | Description            |
|--------|------------------------------|-----------------|------------------------|
| GET    | /api/transactions            | All             | List (filter+paginate) |
| GET    | /api/transactions/{id}       | All             | Get by ID              |
| POST   | /api/transactions            | Admin, Analyst  | Create                 |
| PUT    | /api/transactions/{id}       | Admin, Analyst  | Update                 |
| DELETE | /api/transactions/{id}       | Admin           | Soft-delete            |

### Dashboard
| Method | Endpoint                       | Description                      |
|--------|--------------------------------|----------------------------------|
| GET    | /api/dashboard/summary         | Full summary (income/expense/net)|
| GET    | /api/dashboard/trends          | Monthly income vs expense trends |
| GET    | /api/dashboard/categories      | Category breakdown + %           |

---

## Features Implemented

| Feature                     | Implementation                                      |
|-----------------------------|-----------------------------------------------------|
| JWT Authentication          | Access token (60 min) + Refresh token (7 days)     |
| Role-Based Access Control   | `[RequireRole]` filter attribute + middleware       |
| 3-Tier Architecture         | Controller → Service → Repository                  |
| EF Core + MySQL             | Pomelo provider, migrations, seed data             |
| Soft Delete                 | `IsDeleted` + `DeletedAt` on all entities          |
| Pagination                  | `page` + `pageSize` query params                   |
| Search                      | Full-text search across description/category/notes |
| Filtering                   | By type, category, date range                      |
| Rate Limiting               | 100 req/min per IP, custom middleware              |
| Input Validation            | FluentValidation with auto-validation              |
| Global Error Handling       | `ExceptionMiddleware` with proper HTTP codes       |
| Swagger UI                  | Interactive docs at `/` with JWT support           |
| Unit Tests                  | xUnit + Moq + FluentAssertions (20+ test cases)   |
| Password Hashing            | BCrypt                                             |
| CORS                        | Configured for any origin in dev                  |

---

## Creating Additional Users (Admin API)

```http
POST /api/users
Authorization: Bearer <admin-token>

{
  "firstName": "Jane",
  "lastName": "Smith",
  "email": "jane@company.com",
  "password": "SecurePass@1",
  "roleId": 2
}
```

Role IDs: `1` = Admin, `2` = Analyst, `3` = Viewer

---

## Troubleshooting

| Problem | Fix |
|---------|-----|
| MySQL connection refused | Ensure MySQL is running: `sudo service mysql start` |
| `dotnet-ef` not found | Run: `dotnet tool install --global dotnet-ef` |
| Migration errors | Check connection string in appsettings.json |
| 401 on all requests | Add `Bearer ` prefix before token in Swagger Authorize |
| JWT key error | Ensure key is at least 32 characters long |
| Port already in use | Change port in `Properties/launchSettings.json` |
