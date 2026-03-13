using BackOfficeTradeCapture.Api.Models;

namespace BackOfficeTradeCapture.Api.Services
{
    public interface IReportService
    {
        Task<ReportResponse> GetTradeReportAsync(
       DateOnly from, DateOnly to, CancellationToken ct = default);
    }
}
