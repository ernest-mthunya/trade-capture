using BackOfficeTradeCapture.Api.Data;
using BackOfficeTradeCapture.Api.Models;
using BackOfficeTradeCapture.Api.Services;
using BackOfficeTradeCapture.Contracts;
using Microsoft.AspNetCore.Builder;

namespace BackOfficeTradeCapture.Api;
public static class EndpointsExtensions
{
    public static WebApplication AddBackOfficeTradeCaptureEndpoints(this WebApplication app)
    {
        app.MapPost("/trades", async (TradeRequest trade, ITradeService tradeService) =>
        {
            try
            {
                var result = await tradeService.CaptureTradeAsync(trade);
                return Results.Created($"/trades/{result.Id}", result);
            }
            catch (Exception ex)
            {


                // Log the exception here
                return Results.Problem("An error occurred while processing the trade.");
            }
        })
       .WithName("CreateTrade")
       .WithTags("Trades");

        app.MapGet("/reports/trades", async (
            DateOnly from,
            DateOnly to,
            IReportService reportService,
            CancellationToken ct) =>
        {
            if (from > to)
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    ["from"] = ["'from' must be on or before 'to'."]
                });

            try
            {
                var report = await reportService.GetTradeReportAsync(from, to, ct);
                return Results.Ok(report);
            }
            catch (Exception)
            {
                return Results.Problem("An error occurred while generating the report.");
            }
        })
        .WithName("GetTradeReport")
        .WithTags("Reports");

        return app;
    }
}
