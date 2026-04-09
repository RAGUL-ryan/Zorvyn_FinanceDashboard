# Finance Dashboard — Complete Project README

---

## What is this project?

Finance Dashboard is a full-stack web application built to manage financial records across a team with different access levels. Think of it like an internal finance tool where an admin can manage users and transactions, an analyst can view and add records, and a viewer can monitor the dashboard — all with proper login, security, and a clean interface.

The backend is a REST API built in C# (.NET 8), the frontend is React.js, and everything is stored in a MySQL database.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Backend | C# — ASP.NET Core 8 Web API |
| Frontend | React.js 18 + Vite |
| Database | MySQL 8 with Entity Framework Core |
| Authentication | JWT Access Tokens + Refresh Tokens |
| Styling | Tailwind CSS |
| Charts | Recharts |
| Validation | FluentValidation (backend) |
| Testing | xUnit + Moq + FluentAssertions |
| API Docs | Swagger UI |

---

## Project Structure

```
Zorvyn_Assignment/
├── backend/
│   └── financedashboard/
│       ├── FinanceDashboard.API/          ← Controllers, Middleware, Swagger
│       ├── FinanceDashboard.Business/     ← Services, DTOs, Validators
│       ├── FinanceDashboard.Data/         ← Entities, Repositories, EF Core
│       └── FinanceDashboard.Tests/        ← Unit Tests
│
└── finance-dashboard-ui/                  ← React Frontend
    └── src/
        ├── api/                           ← Axios API calls
        ├── components/                    ← Reusable UI components
        ├── context/                       ← Auth state management
        ├── hooks/                         ← Custom React hooks
        ├── pages/                         ← Full page components
        └── utils/                         ← Formatters and helpers
```

---

## Prerequisites — Install these before anything else

| Tool | Version | Download |
|---|---|---|
| .NET SDK | 8.0 or above | https://dotnet.microsoft.com/download |
| Node.js | 18.0 or above | https://nodejs.org |
| MySQL Server | 8.0 or above | https://dev.mysql.com/downloads/mysql |
| MySQL Workbench | Any | https://dev.mysql.com/downloads/workbench |
| Git | Any | https://git-scm.com |

---

## Roles and What Each Can Do

| Action | Admin | Analyst | Viewer |
|---|:---:|:---:|:---:|
| Login / Logout | ✅ | ✅ | ✅ |
| View Dashboard | ✅ | ✅ | ✅ |
| View Transactions | ✅ | ✅ | ✅ |
| Create / Edit Transactions | ✅ | ✅ | ❌ |
| Delete Transactions | ✅ | ❌ | ❌ |
| Manage Users | ✅ | ❌ | ❌ |
| Activate / Deactivate Users | ✅ | ❌ | ❌ |

---

## Part 1 — Backend Setup

### Step 1 — Set up the database in MySQL Workbench

Open MySQL Workbench, connect to your local server, and run this:

```sql
CREATE DATABASE IF NOT EXISTS FinanceDashboard
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;
```

### Step 2 — Configure your database password

Open this file:
```
backend/financedashboard/FinanceDashboard.API/appsettings.json
```

Update the connection string with your actual MySQL password:

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

> Important: If your password contains special characters like `#`, either remove them or wrap the password in single quotes like `Password='Ragulryan1910#';`

### Step 3 — Install the EF Core migration tool

You only need to do this once on your machine:

```powershell
dotnet tool install --global dotnet-ef
```

### Step 4 — Run database migrations

Navigate to the solution root and run:

```powershell
cd backend/financedashboard

dotnet ef migrations add InitialCreate --project FinanceDashboard.Data --startup-project FinanceDashboard.API

dotnet ef database update --project FinanceDashboard.Data --startup-project FinanceDashboard.API
```

This will automatically create all the tables and seed the default admin user into your database.

### Step 5 — Restore NuGet packages and build

```powershell
dotnet restore
dotnet build
```

### Step 6 — Start the backend

```powershell
cd FinanceDashboard.API
dotnet run
```

You should see something like:
```
Now listening on: http://localhost:5044
```

### Step 7 — Verify the backend is working

Open your browser and go to:
```
http://localhost:5044
```

You should see the **Swagger UI** — a full interactive API documentation page. If you see it, the backend is running perfectly.

---

## Part 2 — Frontend Setup

### Step 1 — Install Node dependencies

Open a new terminal window and run:

```powershell
cd finance-dashboard-ui
npm install
```

### Step 2 — Configure the API URL

Open this file:
```
finance-dashboard-ui/.env
```

Make sure it points to wherever your backend is running:

```
VITE_API_BASE_URL=http://localhost:5044
```

> Replace `5044` with whatever port your backend actually started on. You can see this in the terminal where you ran `dotnet run`.

### Step 3 — Start the frontend

```powershell
npm run dev
```

You should see:
```
Local: http://localhost:3000
```

### Step 4 — Open the application

Go to:
```
http://localhost:3000
```

You will see the login page.

---

## Logging In for the First Time

Use the default admin account that was seeded automatically:

```
Email:    admin@finance.com
Password: Admin@123
```

After logging in you will land on the Dashboard page with full admin access.

---

## Running the Tests

Open a terminal in the solution root and run:

```powershell
cd backend/financedashboard
dotnet test --verbosity normal
```

This runs 20+ unit tests covering the Auth, User, and Transaction service layers using mocked repositories. You should see all tests passing.

---

## API Endpoints — Quick Reference

### Authentication
| Method | Endpoint | Auth Required | Description |
|---|---|---|---|
| POST | /api/auth/login | No | Login and receive JWT tokens |
| POST | /api/auth/refresh-token | No | Get a new access token |
| POST | /api/auth/logout | Yes | Logout and clear refresh token |
| GET | /api/auth/me | Yes | Get current user info |

### Users — Admin only
| Method | Endpoint | Description |
|---|---|---|
| GET | /api/users | List all users |
| GET | /api/users/{id} | Get user by ID |
| POST | /api/users | Create a new user |
| PUT | /api/users/{id} | Update user details |
| DELETE | /api/users/{id} | Soft delete a user |
| PATCH | /api/users/{id}/status | Activate or deactivate |
| POST | /api/users/change-password | Change your own password |

### Transactions
| Method | Endpoint | Who Can Use | Description |
|---|---|---|---|
| GET | /api/transactions | Everyone | List with filters and pagination |
| GET | /api/transactions/{id} | Everyone | Get single transaction |
| POST | /api/transactions | Admin, Analyst | Create new transaction |
| PUT | /api/transactions/{id} | Admin, Analyst | Update transaction |
| DELETE | /api/transactions/{id} | Admin only | Soft delete |

### Dashboard
| Method | Endpoint | Description |
|---|---|---|
| GET | /api/dashboard/summary | Income, expenses, balance, recent activity |
| GET | /api/dashboard/trends | Monthly income vs expense chart data |
| GET | /api/dashboard/categories | Category breakdown with percentages |

---

## How to Use the Swagger UI

1. Go to `http://localhost:5044` in your browser
2. Click on `POST /api/auth/login`
3. Click **Try it out**
4. Enter the credentials and click **Execute**
5. Copy the `accessToken` from the response
6. Click the **Authorize** button at the top of the page
7. Type `Bearer ` followed by the token you copied
8. Click **Authorize** and then **Close**
9. Now all endpoints are unlocked and you can test them interactively

---

## Creating Additional Users

Once logged in as admin, use this API call or the Users page in the frontend:

```json
POST /api/users
{
  "firstName": "Jane",
  "lastName":  "Smith",
  "email":     "jane@company.com",
  "password":  "SecurePass@1",
  "roleId":    2
}
```

Role IDs are: `1` for Admin, `2` for Analyst, `3` for Viewer.

Password rules: minimum 8 characters, must include at least one uppercase letter, one digit, and one special character.

---

## Features Implemented

| Feature | Details |
|---|---|
| JWT Authentication | Access token valid for 60 minutes, refresh token valid for 7 days |
| Role-Based Access Control | Enforced at the API level using a custom `[RequireRole]` filter |
| Soft Delete | Records are never permanently removed — `IsDeleted` and `DeletedAt` are set instead |
| Pagination | All transaction lists support `page` and `pageSize` query parameters |
| Search | Search transactions by description, category, or notes |
| Filtering | Filter by type (Income/Expense), category, and date range |
| Rate Limiting | 100 requests per minute per IP address |
| Input Validation | FluentValidation on all create and update operations |
| Global Error Handling | All exceptions return a consistent JSON response with proper HTTP status codes |
| Password Hashing | BCrypt with salt rounds |
| Auto Token Refresh | Frontend silently refreshes the access token when it expires |
| Swagger UI | Full interactive API documentation with JWT support |
| Unit Tests | 20+ tests across Auth, User, and Transaction services |
| Dashboard Analytics | Income totals, expense totals, net balance, monthly trends, and category breakdown |

---

## Known Limitations

**Refresh tokens are stored as plain text.** In a production system you would hash the refresh token before storing it, similar to how passwords are hashed. This was kept simple for clarity.

**Rate limiting is in-memory per server instance.** If you scaled to multiple servers, each instance would have its own counter. A production deployment would use Redis for shared rate limiting.

**No email verification on signup.** Admins create users directly through the API. A real product would send a welcome email with a setup link.

**The duplicate email check ignores soft-deleted users** unless you apply `IgnoreQueryFilters()` in the `GetByEmailAsync` method of `UserRepository.cs`. If you try to create a user with an email that belongs to a previously deleted account, you will get a 500 error instead of a clean validation message. The fix is one line — see the Troubleshooting section below.

---

## Troubleshooting

| Problem | Cause | Fix |
|---|---|---|
| `Access denied for user 'root'` | Wrong MySQL password in connection string | Update `appsettings.json` with correct password |
| `#` in password breaks connection | `#` is treated as a comment in connection strings | Wrap password in single quotes or change the password |
| `ERR_CONNECTION_REFUSED` on login | Backend is not running | Run `dotnet run` in `FinanceDashboard.API` folder |
| Swagger not loading | Backend crashed on startup | Check terminal for errors — usually a DB connection issue |
| `dotnet-ef` not found | EF tools not installed | Run `dotnet tool install --global dotnet-ef` |
| Port already in use | Something else is on port 5044 or 3000 | Run `netstat -ano \| findstr :5044` and kill the process |
| Duplicate email 500 error | Soft-deleted user has the same email | Add `IgnoreQueryFilters()` to `GetByEmailAsync` in `UserRepository.cs` |
| Frontend shows blank page | `.env` pointing to wrong port | Update `VITE_API_BASE_URL` in `.env` to match your backend port |
| Tailwind class not found | Custom color names used in `@apply` | Replace with standard Tailwind colors like `red-600`, `blue-600` |
| 401 on all API calls | Token not attached or expired | Re-login or check that `Bearer ` prefix is included |
| Migration fails | Database doesn't exist yet | Create the database in MySQL Workbench first |

---

## Two Terminals Always Running

This is the most important thing to remember. The frontend and backend are separate applications. You need both running at the same time:

**Terminal 1 — Backend:**
```powershell
cd backend/financedashboard/FinanceDashboard.API
dotnet run
```

**Terminal 2 — Frontend:**
```powershell
cd finance-dashboard-ui
npm run dev
```

Then open `http://localhost:3000` in your browser.

---

## Default Credentials Summary

| Account | Email | Password | Role |
|---|---|---|---|
| System Admin | admin@finance.com | Admin@123 | Admin |
