# BackOfficeTradeCapture — Solution Notes

## API and Schema Choices

### REST API
The API is built with ASP.NET Core Minimal APIs using strongly-typed results (`TypedResults`) throughout. Endpoints are organised in a static extension class (`EndpointsExtensions`) to keep `Program.cs` clean.

Three endpoints are exposed:

| Method | Route | Purpose |
|--------|-------|---------|
| `POST` | `/trades` | Capture a single trade |
| `POST` | `/trades/batch` | Capture multiple trades atomically |
| `GET` | `/reports/trades?from=&to=` | Aggregate trade report by account and symbol |

Query parameters on the report endpoint are accepted as `string` and parsed to `DateOnly` so that invalid dates return a structured `ValidationProblem` rather than a 400 with no detail.

### Schema

The `Trades` table maps to `TradeEntity`:

| Column | Type | Notes |
|--------|------|-------|
| `Id` | `int` PK | Auto-increment surrogate key |
| `ExternalId` | `nvarchar(50)` | Unique — enforced by `UX_Trades_ExternalId` |
| `Account` | `nvarchar(50)` | |
| `Symbol` | `nvarchar(20)` | |
| `Side` | `nvarchar(4)` | `BUY` or `SELL` |
| `Quantity` | `int` | |
| `Price` | `decimal(18,4)` | |
| `TradeTime` | `datetimeoffset` | Stored with UTC offset |
| `Currency` | `nchar(3)` | ISO 4217 |
| `NotionalBase` | `decimal(18,2)` | Calculated at ingestion time (Quantity × Price × FX rate) |
| `BaseCurrency` | `nchar(3)` | Always `EUR` |
| `CreatedAt` | `datetimeoffset` | Set to `UtcNow` at insert |

Indexes:

- `UX_Trades_ExternalId` — unique, supports idempotency checks
- `IX_Trades_TradeTime` — supports date range queries
- `IX_Trades_Account_Symbol` — supports grouping in reports
- `IX_Trades_TradeTime_Account_Symbol` — composite covering index for the report stored procedure

### Report Stored Procedure

`usp_GetTradeReport` accepts `@From` and `@To` as `DATETIME2` (mapped from `DateOnly` by EF Core as `DbType.Date`). It aggregates all trades within the inclusive calendar date range, grouping by account and symbol, and returns one summary row per group.

#### Full Definition

```sql
USE [TradeDb]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[usp_GetTradeReport]
    @From DATETIME2,
    @To   DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        t.Account                                                        AS Account,
        t.Symbol                                                         AS Symbol,
        SUM(t.Quantity)                                                  AS TotalQty,
        SUM(CAST(t.Quantity AS DECIMAL(18,6)) * t.Price)
            / SUM(CAST(t.Quantity AS DECIMAL(18,6)))                     AS AvgPrice,
        SUM(t.NotionalBase)                                              AS NotionalBase,
        MAX(t.BaseCurrency)                                              AS BaseCcy
    FROM dbo.Trades t
    WHERE CAST(t.TradeTime AS DATE) >= CAST(@From AS DATE)
      AND CAST(t.TradeTime AS DATE) <= CAST(@To AS DATE)
    GROUP BY t.Account, t.Symbol
    ORDER BY t.Account, t.Symbol;
END;
```

#### Explanation

**Parameters**

| Parameter | Type | Description |
|-----------|------|-------------|
| `@From` | `DATETIME2` | Start of the date range (inclusive) |
| `@To` | `DATETIME2` | End of the date range (inclusive) |

Both are passed as `DATETIME2` but effectively treated as dates — the `CAST(... AS DATE)` in the `WHERE` clause strips any time component, so the caller does not need to zero out the time portion before calling.

**Output columns**

| Column | Expression | Description |
|--------|------------|-------------|
| `Account` | `t.Account` | Trading account identifier |
| `Symbol` | `t.Symbol` | Instrument/ticker |
| `TotalQty` | `SUM(Quantity)` | Total units traded across all matching trades |
| `AvgPrice` | Quantity-weighted average | Weighted average price: total value divided by total quantity, cast to `DECIMAL(18,6)` to avoid integer division |
| `NotionalBase` | `SUM(NotionalBase)` | Total notional value in the base currency (EUR) |
| `BaseCcy` | `MAX(BaseCurrency)` | Base currency — `MAX` is used as an aggregate no-op since all rows carry the same value (`EUR`) |

**Date filtering**

```sql
WHERE CAST(t.TradeTime AS DATE) >= CAST(@From AS DATE)
  AND CAST(t.TradeTime AS DATE) <= CAST(@To AS DATE)
```

`TradeTime` is stored as `datetimeoffset` with a UTC offset. Casting both sides to `DATE` strips the time-of-day and UTC offset, so trades are matched by calendar date regardless of when during the day they occurred. This avoids off-by-one issues that would arise from comparing raw `datetimeoffset` values against midnight boundaries.

The composite index `IX_Trades_TradeTime_Account_Symbol` covers this query — the leading `TradeTime` column supports the range scan, and `Account`/`Symbol` are included to avoid a key lookup for the `GROUP BY`.

**Grouping and ordering**

Results are grouped and ordered by `Account` then `Symbol`, producing one aggregated row per account–symbol pair within the requested date range.

#### Full Definition

```sql
USE [TradeDb]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[usp_GetTradeReport]
    @From DATETIME2,
    @To   DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        t.Account                                                        AS Account,
        t.Symbol                                                         AS Symbol,
        SUM(t.Quantity)                                                  AS TotalQty,
        SUM(CAST(t.Quantity AS DECIMAL(18,6)) * t.Price)
            / SUM(CAST(t.Quantity AS DECIMAL(18,6)))                     AS AvgPrice,
        SUM(t.NotionalBase)                                              AS NotionalBase,
        MAX(t.BaseCurrency)                                              AS BaseCcy
    FROM dbo.Trades t
    WHERE CAST(t.TradeTime AS DATE) >= CAST(@From AS DATE)
      AND CAST(t.TradeTime AS DATE) <= CAST(@To AS DATE)
    GROUP BY t.Account, t.Symbol
    ORDER BY t.Account, t.Symbol;
END;
```

---

## Idempotency and Concurrency

### Same trade submitted again (idempotency)

Every trade request carries an `ExternalId` supplied by the caller. Before inserting, `CaptureTradeInternalAsync` issues a locking read:

```sql
SELECT * FROM Trades WITH (UPDLOCK, ROWLOCK) WHERE ExternalId = @externalId
```

If a row is found the existing record is returned immediately — no duplicate is written and the response is identical to the original. This makes `POST /trades` safe to retry.

`UPDLOCK` upgrades the shared read lock to an update lock, preventing a second concurrent reader from also finding no row and both proceeding to insert.

### Concurrent submissions of the same trade

Two requests arriving simultaneously for the same `ExternalId` will both acquire a shared lock on the index scan. `UPDLOCK` means only one can proceed to the insert phase at a time. The second request will block, then re-read and find the row already inserted by the first, and return the existing record.

As a second line of defence the unique index `UX_Trades_ExternalId` will reject any duplicate that slips through, and the exception propagates as a 500 rather than a silent duplicate.

### Batch atomicity

`POST /trades/batch` wraps all inserts in a single database transaction. If any trade in the batch fails (validation is rejected upfront; any DB error during insert triggers a rollback), no trades from the batch are persisted. The entire batch is re-submittable.

---

## SOAP / WCF Currency Rate Integration

### Current approach — in-process stub

`ICurrencyRateService` is defined as a `[ServiceContract]` in the shared `BackOfficeTradeCapture.Contracts` project. The current implementation (`CurrencyRateService`) is a simple in-process stub with hardcoded rates:

| Currency | Rate to EUR |
|----------|-------------|
| USD | 0.92 |
| GBP | 1.17 |
| EUR | 1.00 |
| JPY | 0.0062 |

This is registered in DI as a scoped service and injected into `TradeService`. No network call is made — the stub exists so the interface contract is in place and the rest of the system can be built and tested without a live SOAP endpoint.

### How a real SOAP endpoint would be wired in

1. **Generate the client proxy** using `dotnet-svcutil`:

```bash
dotnet tool install --global dotnet-svcutil
dotnet-svcutil https://currency-service/CurrencyRate.svc?wsdl
```

This produces a typed client class (e.g. `CurrencyRateServiceClient`) and a `BasicHttpBinding` configuration.

2. **Implement the interface** wrapping the generated client:

```csharp
public class SoapCurrencyRateService : ICurrencyRateService
{
    private readonly CurrencyRateServiceClient _client;

    public SoapCurrencyRateService(CurrencyRateServiceClient client)
    {
        _client = client;
    }

    public decimal GetRate(string fromCurrency, string toCurrency)
        => _client.GetRateAsync(fromCurrency, toCurrency).GetAwaiter().GetResult();
}
```

3. **Register in DI** replacing the stub:

```csharp
builder.Services.AddScoped<ICurrencyRateService, SoapCurrencyRateService>();
builder.Services.AddScoped(_ =>
    new CurrencyRateServiceClient(
        new BasicHttpBinding(),
        new EndpointAddress(builder.Configuration["CurrencyService:Url"])));
```

4. **Configuration** in `appsettings.json`:

```json
"CurrencyService": {
  "Url": "https://currency-service/CurrencyRate.svc"
}
```

No other changes are required — `TradeService` depends on `ICurrencyRateService` and is unaware of whether the implementation is a stub or a live SOAP client.

---

## Seed Data

To run the demo quickly, execute the following against your LocalDB or Developer Edition instance:

```sql
USE [TradeDb];

INSERT INTO Trades (ExternalId, Account, Symbol, Side, Quantity, Price, TradeTime, Currency, NotionalBase, BaseCurrency, CreatedAt)
VALUES
  ('TXN-2026-0001', 'ACC-PRIMARY-01', 'BTC',  'BUY',  1000, 55000.50, '2026-03-14T10:00:00+00:00', 'USD', 50600460.00, 'EUR', SYSDATETIMEOFFSET()),
  ('TXN-2026-0002', 'ACC-PRIMARY-01', 'ETH',  'SELL',  500,  3000.00, '2026-03-14T11:00:00+00:00', 'EUR',  1380000.00, 'EUR', SYSDATETIMEOFFSET()),
  ('TXN-2026-0003', 'ACC-PRIMARY-02', 'AAPL', 'BUY',   200,   178.25, '2026-03-14T12:30:00+00:00', 'USD',    32782.00, 'EUR', SYSDATETIMEOFFSET()),
  ('TXN-2026-0004', 'ACC-PRIMARY-02', 'MSFT', 'BUY',   150,   415.75, '2026-03-14T13:00:00+00:00', 'USD',    57374.70, 'EUR', SYSDATETIMEOFFSET()),
  ('TXN-2026-0005', 'ACC-PRIMARY-03', 'GOLD', 'SELL',  300,  2050.00, '2026-03-14T14:15:00+00:00', 'GBP',   718785.00, 'EUR', SYSDATETIMEOFFSET());
```

Then verify the report endpoint:

```
GET /reports/trades?from=2026-03-14&to=2026-03-14
```
