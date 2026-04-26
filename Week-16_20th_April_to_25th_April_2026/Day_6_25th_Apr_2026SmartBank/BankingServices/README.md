# Banking Services – Loan & Card

.NET 10 microservices for **Loan Service** and **Card Service** with SQL Server, JWT auth, and Swagger UI.

---

## Project Structure

```
BankingServices/
├── BankingServices.sln
├── LoanService/
│   ├── Controllers/       LoanController.cs
│   ├── Data/              LoanDbContext.cs
│   ├── DTOs/              LoanDTOs.cs
│   ├── Middleware/        JwtMiddleware.cs
│   ├── Migrations/        InitialCreate.sql
│   ├── Models/            Loan.cs, EmiPlan.cs
│   ├── Services/          ILoanService.cs, LoanServiceImpl.cs
│   ├── appsettings.json
│   └── Program.cs
└── CardService/
    ├── Controllers/       CardController.cs
    ├── Data/              CardDbContext.cs
    ├── DTOs/              CardDTOs.cs
    ├── Middleware/        JwtMiddleware.cs
    ├── Migrations/        InitialCreate.sql
    ├── Models/            Card.cs
    ├── Services/          ICardService.cs, CardServiceImpl.cs
    ├── appsettings.json
    └── Program.cs
```

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (local or Docker)
- (Optional) `dotnet-ef` CLI: `dotnet tool install --global dotnet-ef`

---

## Quick Start

### 1. Configure SQL Server

Edit `appsettings.json` in both services:

```json
"ConnectionStrings": {
  "LoanDb": "Server=localhost;Database=LoanDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;"
}
```

### 2. Apply Database Migrations

**Option A – EF Core CLI (recommended):**
```bash
cd LoanService
dotnet ef migrations add InitialCreate
dotnet ef database update

cd ../CardService
dotnet ef migrations add InitialCreate
dotnet ef database update
```

**Option B – Run the SQL scripts manually:**
```
LoanService/Migrations/InitialCreate.sql
CardService/Migrations/InitialCreate.sql
```

### 3. Update JWT Secret

In both `appsettings.json`, replace:
```json
"Key": "YourSuperSecretKeyHereAtLeast32Chars!!"
```
with a strong secret (≥ 32 characters).

### 4. Run the Services

```bash
# Terminal 1
cd LoanService
dotnet run
# Swagger → http://localhost:5000

# Terminal 2
cd CardService
dotnet run
# Swagger → http://localhost:5001
```

---

## Swagger / OpenAPI

Both services expose Swagger UI at their root (`/`):

| Service      | URL                          |
|--------------|------------------------------|
| Loan Service | http://localhost:5000         |
| Card Service | http://localhost:5001         |

To authenticate in Swagger:
1. Generate a JWT token (see section below).
2. Click **Authorize** → enter `Bearer <your_token>`.

---

## JWT Token Generation (test helper)

Use any JWT tool or add a quick Auth endpoint. Sample payload:

```json
{
  "sub": "user-guid",
  "name": "AdminUser",
  "role": "Admin",
  "iss": "BankingApp",
  "aud": "BankingApp",
  "exp": <unix_timestamp>
}
```

Roles used:
- `Admin` — full access (approve loans, issue/block cards)
- `Customer` — apply loan, repay EMI, block own card, PIN reset

---

## API Endpoints

### Loan Service (`/api/v1/loans`)

| Method | Route                            | Role          | Description               |
|--------|----------------------------------|---------------|---------------------------|
| POST   | `/apply`                         | Customer/Admin| Apply for a loan          |
| POST   | `/decision`                      | Admin         | Approve or reject a loan  |
| GET    | `/{loanId}`                      | Any           | Get loan + EMI schedule   |
| GET    | `/{loanId}/schedule`             | Any           | Get EMI schedule only     |
| GET    | `/customer/{customerId}`         | Any           | Get all loans for customer|
| POST   | `/repay`                         | Customer/Admin| Record EMI repayment      |

### Card Service (`/api/v1/cards`)

| Method | Route                            | Role          | Description               |
|--------|----------------------------------|---------------|---------------------------|
| POST   | `/debit`                         | Admin         | Issue debit card          |
| POST   | `/credit`                        | Admin         | Issue credit card         |
| POST   | `/block`                         | Customer/Admin| Block a card              |
| POST   | `/pin-reset`                     | Customer/Admin| Reset card PIN            |
| GET    | `/{cardId}`                      | Any           | Get card by ID            |
| GET    | `/customer/{customerId}`         | Any           | Get all cards for customer|

---

## Integration Points

| Integration        | Used by          | Purpose                              |
|--------------------|------------------|--------------------------------------|
| Customer Service   | Both             | Validate customer profile            |
| Account Service    | Both             | Salary / balance validation          |
| Notification Service | Both           | Approval / block alerts (stub ready) |
| Audit Service      | Both             | Admin action logging                 |

> Integration stubs are logged via `ILogger`. Wire up HTTP clients or message bus as needed.

---

## Tech Stack

| Layer        | Technology                          |
|--------------|-------------------------------------|
| Runtime      | .NET 10 / ASP.NET Core              |
| ORM          | Entity Framework Core 9 (SQL Server)|
| Auth         | JWT Bearer (Microsoft.Identity)     |
| Docs         | Swashbuckle / Swagger UI            |
| Database     | SQL Server (LoanDB, CardDB)         |
