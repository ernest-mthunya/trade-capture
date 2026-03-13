using BackOfficeTradeCapture.Api.Data;
using BackOfficeTradeCapture.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BackOfficeTradeCapture.Api.Services
{
    public class ReportService(BackOfficeDbContext db) : IReportService
    {
        public async Task<ReportResponse> GetTradeReportAsync(
        DateOnly from, DateOnly to, CancellationToken ct = default)
        {
            var fromDt = from.ToDateTime(TimeOnly.MinValue);
            // +1 day makes the upper bound exclusive, capturing all
            // intraday timestamps on the 'to' date
            var toDt = to.AddDays(1).ToDateTime(TimeOnly.MinValue);

            var rows = await db.TradeReportRows
                .FromSqlRaw(
                    "EXEC dbo.usp_GetTradeReport @From = {0}, @ToExclusive = {1}",
                    fromDt, toDt)
                .ToListAsync(ct);

            return new ReportResponse
            {
                From = from,
                To = to,
                BaseCcy = "EUR",
                Rows = rows
            };
        }
    }
}
