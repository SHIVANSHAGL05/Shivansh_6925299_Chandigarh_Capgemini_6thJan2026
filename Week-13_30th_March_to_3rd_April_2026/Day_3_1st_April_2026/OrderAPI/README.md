# Order Processing API – Case Study 2

## Project Structure

```
OrderProcessing.sln
├── OrderProcessing.API/                    ← ASP.NET Core Web API
│   ├── Models/Order.cs                     ← Domain model
│   ├── Services/IOrderService.cs           ← Interface
│   ├── Services/OrderService.cs            ← Implementation
│   ├── Controllers/OrderController.cs      ← HTTP layer
│   └── Program.cs
│
├── OrderProcessing.Tests.xUnit/            ← xUnit + Moq
│   └── OrderControllerTests.cs
│
└── OrderProcessing.Tests.NUnit/            ← NUnit + Moq
    └── OrderControllerTests.cs
```

## Endpoint

| Method | Route       | Success        | Failure         |
|--------|-------------|----------------|-----------------|
| POST   | /api/order  | 201 Created    | 400 Bad Request |

## Run Commands

```bash
# Run all tests
dotnet test

# Run API on a different port (port 5000 may be blocked)
dotnet run --project OrderProcessing.API/ --urls "http://localhost:5050"
# Then browse: http://localhost:5050/swagger
```

## xUnit vs NUnit — Side-by-Side

| Concept          | xUnit                          | NUnit                            |
|------------------|--------------------------------|----------------------------------|
| Setup            | Constructor                    | `[SetUp]` method                 |
| Single test      | `[Fact]`                       | `[Test]`                         |
| Parameterised    | `[Theory]` + `[InlineData]`    | `[TestCase(...)]`                |
| Assert 201       | `Assert.IsType<StatusCodeResult>` | `Assert.That(..., Is.InstanceOf<StatusCodeResult>())` |
| Assert 400       | `Assert.IsType<BadRequestResult>` | `Assert.That(..., Is.InstanceOf<BadRequestResult>())`  |
| Verify mock      | `_mock.Verify(..., Times.Once)` | same — Moq is framework-agnostic |

## Key Learning Points

1. **Mocking return values** — `ReturnsAsync(true)` / `ReturnsAsync(false)` lets us
   control both positive and negative paths without any real database.
2. **201 vs 400** — `StatusCode(201)` returns `StatusCodeResult`;
   `BadRequest()` returns `BadRequestResult`. Both are `IActionResult`.
3. **Verify** — confirms the service method was actually called, not just that
   the controller returned the right HTTP code.
4. **Constructor vs [SetUp]** — xUnit creates a fresh instance per test automatically;
   NUnit reuses the instance so `[SetUp]` must reset shared state manually.
