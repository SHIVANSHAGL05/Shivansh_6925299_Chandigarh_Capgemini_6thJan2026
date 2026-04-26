# Employee Management Portal
### Hands-on Assignment – 19th Mar 2026

---

## Tech Stack
- **ASP.NET Core MVC** (.NET 9)
- **Entity Framework Core** (SQL Server)
- **ASP.NET Identity** (Authentication + Roles)
- **Bootstrap 5** (Responsive UI)

---

## Project Structure

```
EmployeePortal/
├── Controllers/
│   ├── HomeController.cs         ← Home, Dashboard, Profile pages
│   └── EmployeeController.cs     ← Full CRUD with role protection
├── Data/
│   ├── ApplicationDbContext.cs   ← EF Core DB context
│   └── SeedData.cs               ← Seeds roles, admin user, sample employees
├── Migrations/
│   └── ...                       ← EF Core migration files
├── Models/
│   └── Employee.cs               ← Employee model with validation
├── Views/
│   ├── Employee/
│   │   ├── Index.cshtml          ← List with search, pagination, delete modal
│   │   ├── Create.cshtml         ← Add employee form
│   │   └── Edit.cshtml           ← Edit employee form
│   ├── Home/
│   │   ├── Index.cshtml          ← Landing page
│   │   ├── Dashboard.cshtml      ← Stats dashboard
│   │   └── Profile.cshtml        ← Logged-in user profile
│   └── Shared/
│       ├── _Layout.cshtml        ← Main layout with navbar
│       └── _ValidationScriptsPartial.cshtml
├── Areas/Identity/               ← ASP.NET Identity UI (scaffolded)
├── Program.cs                    ← App startup + DI configuration
├── appsettings.json              ← Connection string
└── EmployeePortal.csproj
```

---

## Setup Instructions

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB is fine – installed with Visual Studio)
- Visual Studio 2022 or VS Code

### Step 1 – Restore & Run

```bash
cd EmployeePortal
dotnet restore
dotnet run
```

> Open browser at `http://localhost:5000`

### Step 2 – Database Migration

The app auto-applies migrations on startup via `SeedData.Initialize()`.

If you want to apply manually:

```bash
dotnet ef database update
```

To add a new migration later:

```bash
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

---

## Seeded Credentials

| Role     | Email             | Password  |
|----------|-------------------|-----------|
| Admin    | admin@portal.com  | Admin@123 |
| Employee | emp@portal.com    | Emp@123   |

---

## Features Implemented

### Authentication & Authorization
- [x] Register / Login / Logout via ASP.NET Identity
- [x] Two roles: **Admin** and **Employee**
- [x] `[Authorize]` and `[Authorize(Roles = "Admin")]` on controller actions

### Role-Based Access

| Feature         | Admin | Employee |
|----------------|-------|----------|
| View Employees  | ✅    | ✅       |
| Add Employee    | ✅    | ❌       |
| Edit Employee   | ✅    | ❌       |
| Delete Employee | ✅    | ❌       |

### Employee Module
- [x] Fields: Id, Name, Department, Salary
- [x] View list in Bootstrap table
- [x] Add Employee (Admin only, with validation)
- [x] Edit Employee (Admin only)
- [x] Delete Employee (Admin only, Bootstrap modal confirmation)

### UI
- [x] Responsive Bootstrap 5 layout
- [x] Navbar with Home, Employees, Dashboard, Login/Logout
- [x] Logged-in username + role badge in navbar
- [x] Success toast messages via TempData

### Search
- [x] Filter employees by **Name** or **Department**

### Profile Page
- [x] Shows logged-in user's email, username, and roles

### Bonus Tasks
- [x] Bootstrap Modal for delete confirmation
- [x] Pagination (5 employees per page)
- [x] Dashboard (Total Employees, Total Departments)
- [x] Seed Data (roles, admin user, sample employees auto-created)
- [x] Validation (Required fields, Salary > 0)

---

## Security Notes
- All employee routes require `[Authorize]`
- Create, Edit, Delete require `[Authorize(Roles = "Admin")]`
- Anti-forgery tokens on all POST forms
- Passwords hashed by ASP.NET Identity

---

## Scaffolding Identity Pages (Optional)

If you want to customize the Login/Register pages:

```bash
dotnet aspnet-codegenerator identity -dc EmployeePortal.Data.ApplicationDbContext --files "Account.Login;Account.Register"
```
