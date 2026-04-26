# ASP.NET Core Web API + Razor Pages Case Study

## Projects

| Project | Port | Purpose |
|---|---|---|
| `ProductApi` | http://localhost:5000 | REST Web API |
| `ProductClient` | http://localhost:5001 | Razor Pages frontend |

---

## Quick Start

### 1. Run the API
```bash
cd ProductApi
dotnet run
```
- Swagger UI: http://localhost:5000/swagger

### 2. Run the Client (new terminal)
```bash
cd ProductClient
dotnet run
```
- Home: http://localhost:5001
- Products page: http://localhost:5001/Products

---

## API Endpoints

| Method | URL | Description |
|---|---|---|
| GET | /api/products | List all products |
| GET | /api/products/{id} | Get by ID |
| POST | /api/products | Create product |
| PUT | /api/products/{id} | Update product |
| DELETE | /api/products/{id} | Delete product |

---

## Features
- Full CRUD from the Razor Pages UI (no Swagger needed)
- HttpClient factory with named client
- Bootstrap 5 responsive design
- In-memory data store (static list in controller)
- CORS configured for cross-origin requests between the two projects

---

## Requirements
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
