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

            var rows = await db.TradeReportRows
                .FromSqlRaw(
                    "EXEC dbo.usp_GetTradeReport @From = {0}, @To = {1}",
                    from, to)
                .AsNoTracking()
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
