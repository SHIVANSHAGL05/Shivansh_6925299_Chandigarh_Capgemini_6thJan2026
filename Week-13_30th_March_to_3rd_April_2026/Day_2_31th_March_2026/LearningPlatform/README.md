# LearnHub – Learning Platform (Separated Architecture)

## Project Structure

```
LearningPlatform.sln
├── LearningPlatform.API/        ← Pure REST API (ASP.NET Core Web API)
├── LearningPlatform.Web/        ← Pure Frontend (ASP.NET Core Razor Pages)
└── LearningPlatform.Tests/      ← Unit & Integration Tests
```

## Architecture

```
[Browser]
    │
    ▼
[LearningPlatform.Web]   ← Razor Pages (https://localhost:7101)
    │  HttpClient calls
    ▼
[LearningPlatform.API]   ← REST API with JWT (https://localhost:7100)
    │  EF Core
    ▼
[SQL Server]             ← LearningPlatformDb
```

- The **Web** project never touches the database directly — all data goes through the API.
- The **API** issues JWT tokens; the Web project stores them in **session** and sends them as Bearer tokens on each API call.
- Cookie authentication in the Web project controls page-level `[Authorize]`.

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB, Express, or full) — or update the connection string
- EF Core CLI tools: `dotnet tool install --global dotnet-ef`
- Visual Studio 2022 / VS Code / Rider

---

## Getting Started

### Step 1 — Configure the API

Edit `LearningPlatform.API/appsettings.json` if needed:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=LearningPlatformDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "CHANGE_THIS_TO_A_STRONG_SECRET_32_CHARS_MIN!!",
    "Issuer": "LearningPlatform",
    "Audience": "LearningPlatformUsers"
  },
  "AllowedOrigins": [
    "https://localhost:7101",
    "http://localhost:5101"
  ]
}
```

### Step 2 — Configure the Web project

Edit `LearningPlatform.Web/appsettings.json` if needed:

```json
{
  "ApiSettings": {
    "BaseUrl": "http://localhost:5100/"
  }
}
```

> ⚠️ Use the HTTP port (5100) not HTTPS (7100) unless you have a dev certificate set up. Check which port your API printed on startup and match it here.

---

### Step 3 — Create the Database Migration (ONE TIME ONLY)

> ⚠️ This step is required **once** before the very first run.
> After this, migrations are applied **automatically** every time the API starts — you never need to run `dotnet ef database update` manually.

```bash
cd LearningPlatform.API
dotnet ef migrations add InitialCreate
```

This creates a `Migrations/` folder with the full database schema.

**If you later change any Model** (e.g. add a new property to `Course.cs`), run:

```bash
dotnet ef migrations add <DescriptiveName>
# e.g. dotnet ef migrations add AddCourseImageUrl
```

Then just restart the API — it applies the new migration automatically.

---

### Step 4 — Run both projects

**Option A – Visual Studio (recommended)**
1. Right-click the Solution → *Set Startup Projects* → *Multiple startup projects*
2. Set both `LearningPlatform.API` and `LearningPlatform.Web` to **Start**
3. Press **F5**

**Option B – Terminal (two separate terminals)**

```bash
# Terminal 1 – API  (Swagger UI opens automatically)
cd LearningPlatform.API
dotnet run
# → https://localhost:7100

# Terminal 2 – Web
cd LearningPlatform.Web
dotnet run
# → https://localhost:7101
```

---

### Step 5 — What happens on first run

When the API starts for the first time it will automatically:

1. ✅ Apply the `InitialCreate` migration → creates all tables in SQL Server
2. ✅ Seed demo accounts and a sample course → no manual inserts needed

**Seeded demo accounts:**

| Role       | Email                        | Password          |
|------------|------------------------------|-------------------|
| Admin      | admin@learnhub.com           | Admin@123         |
| Instructor | instructor@learnhub.com      | Instructor@123    |
| Student    | student@learnhub.com         | Student@123       |

---

## Ports at a Glance

| Project               | HTTPS                  | HTTP                  |
|-----------------------|------------------------|-----------------------|
| LearningPlatform.API  | https://localhost:7100 | http://localhost:5100 |
| LearningPlatform.Web  | https://localhost:7101 | http://localhost:5101 |

> If either port is already in use, edit the `applicationUrl` in the respective `Properties/launchSettings.json`.

---

## API Endpoints

| Method | Route                          | Auth          | Description              |
|--------|--------------------------------|---------------|--------------------------|
| POST   | /api/auth/register             | —             | Register new user        |
| POST   | /api/auth/login                | —             | Login → JWT token        |
| POST   | /api/auth/refresh              | —             | Refresh JWT token        |
| GET    | /api/v1/courses                | —             | List / search courses    |
| GET    | /api/v1/courses/{id}           | —             | Get course by id         |
| POST   | /api/v1/courses                | Instructor    | Create course            |
| PUT    | /api/v1/courses/{id}           | Instructor    | Update course            |
| DELETE | /api/v1/courses/{id}           | Admin         | Delete course            |
| GET    | /api/v1/courses/{id}/lessons   | —             | Get lessons for course   |
| POST   | /api/v1/courses/{id}/lessons   | Instructor    | Add lesson to course     |
| POST   | /api/v1/enroll                 | Student/Admin | Enroll in course         |
| GET    | /api/v1/enroll/my              | Any auth      | My enrollments           |
| DELETE | /api/v1/enroll/{courseId}      | Student/Admin | Unenroll                 |

---

## Web Pages

| URL                    | Access      | Description              |
|------------------------|-------------|--------------------------|
| /Courses/Index         | Public      | Browse & search courses  |
| /Courses/Detail?id=N   | Public      | Course detail + enroll   |
| /Courses/Create        | Instructor  | Create a new course      |
| /Student/MyEnrollments | Logged in   | My enrolled courses      |
| /Auth/Login            | Public      | Login                    |
| /Auth/Register         | Public      | Register                 |
| /Auth/Logout           | Logged in   | Logout                   |

---

## Hosting (Production)

### API → Azure App Service / IIS
- Publish `LearningPlatform.API` as a Web App
- Set `AllowedOrigins` to your Web app's URL

### Web → Azure App Service / IIS
- Publish `LearningPlatform.Web` as a separate Web App
- Set `ApiSettings:BaseUrl` to your deployed API URL

### Environment Variables (Production)
```
Jwt__Key=<strong-secret>
ConnectionStrings__DefaultConnection=<prod-connection-string>
AllowedOrigins__0=https://your-web-app.azurewebsites.net
ApiSettings__BaseUrl=https://your-api-app.azurewebsites.net/
```
