# BackOfficeTradeCapture

A .NET 8 back-office trade capture system consisting of three projects.

## Solution Structure

| Project | SDK | Purpose |
|---------|-----|---------|
| `BackOfficeTradeCapture.Api` | `Microsoft.NET.Sdk.Web` | REST API for trade capture and reporting |
| `BackOfficeTradeCapture.Wcf` | `Microsoft.NET.Sdk.Web` | CoreWCF host exposing the currency rate service |
| `BackOfficeTradeCapture.Contracts` | `Microsoft.NET.Sdk` | Shared service contracts and interfaces |

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server Express or Developer Edition

## Getting Started

### 1. Restore packages

From the solution root:

```bash
dotnet restore
```

### 2. Database setup

Update the connection string in `BackOfficeTradeCapture.Api/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=TradeDb;Trusted_Connection=True;TrustServerCertificate=True"
}
```

For SQL Server Express replace the server with `<YOUR_MACHINE>\\SQLEXPRESS`.

### 3. Apply migrations

```bash
dotnet ef database update --project BackOfficeTradeCapture.Api
```

### 4. Run the solution

Both `BackOfficeTradeCapture.Api` and `BackOfficeTradeCapture.Wcf` must run simultaneously — the API calls the WCF service to resolve currency rates at trade ingestion time.

### Visual Studio

1. Right-click the solution → **Set Startup Projects**
2. Select **Multiple startup projects**
3. Set both `BackOfficeTradeCapture.Api` and `BackOfficeTradeCapture.Wcf` to **Start**
4. Press **F5**

### CLI

Open two terminals from the solution root:

**Terminal 1 — WCF host:**
```bash
dotnet run --project BackOfficeTradeCapture.Wcf
```

**Terminal 2 — API:**
```bash
dotnet run --project BackOfficeTradeCapture.Api
```

## Endpoints

Once running, Swagger UI is available at:

```
https://localhost:7269/swagger
```

| Method | Route | Description |
|--------|-------|-------------|
| `POST` | `/trades` | Capture a single trade |
| `POST` | `/trades/batch` | Capture multiple trades atomically |
| `GET` | `/reports/trades?from=YYYY-MM-DD&to=YYYY-MM-DD` | Aggregate trade report by account and symbol |

### Example — single trade

```http
POST /trades
Content-Type: application/json

{
  "external_id": "TXN-2026-0001",
  "account": "ACC-PRIMARY-01",
  "symbol": "BTC",
  "side": "BUY",
  "quantity": 1000,
  "price": 55000.50,
  "trade_time": "2026-03-14T10:00:00Z",
  "currency": "USD"
}
```

### Example — batch

```http
POST /trades/batch
Content-Type: application/json

{
  "trades": [
    {
      "external_id": "TXN-2026-0002",
      "account": "ACC-PRIMARY-01",
      "symbol": "ETH",
      "side": "SELL",
      "quantity": 500,
      "price": 3000.00,
      "trade_time": "2026-03-14T11:00:00Z",
      "currency": "EUR"
    },
    {
      "external_id": "TXN-2026-0003",
      "account": "ACC-PRIMARY-02",
      "symbol": "AAPL",
      "side": "BUY",
      "quantity": 200,
      "price": 178.25,
      "trade_time": "2026-03-14T12:30:00Z",
      "currency": "USD"
    }
  ]
}
```

### Example — report

```
GET /reports/trades?from=2026-03-14&to=2026-03-14
```

## NuGet Packages

### BackOfficeTradeCapture.Api
| Package | Version |
|---------|---------|
| `Microsoft.EntityFrameworkCore.SqlServer` | 8.0.0 |
| `Microsoft.EntityFrameworkCore.Tools` | 8.0.0 |
| `Swashbuckle.AspNetCore` | 6.6.2 |
| `System.ServiceModel.Http` | 8.1.1 |
| `System.ServiceModel.Primitives` | 8.1.1 |

### BackOfficeTradeCapture.Wcf
| Package | Version |
|---------|---------|
| `CoreWCF.Http` | 1.8.0 |
| `CoreWCF.Primitives` | 1.8.0 |

### BackOfficeTradeCapture.Contracts
| Package | Version |
|---------|---------|
| `System.ServiceModel.Primitives` | 8.1.1 |

## Further Reading

See [SOLUTION.md](./SOLUTION.md) for a detailed explanation of API and schema design choices, idempotency and concurrency handling, and how the SOAP/WCF integration works.
