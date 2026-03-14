using BackOfficeTradeCapture.Api.Data;
using BackOfficeTradeCapture.Api.Models;
using BackOfficeTradeCapture.Api.Services;
using BackOfficeTradeCapture.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BackOfficeTradeCapture.Api;
public static class EndpointsExtensions
{
    public static WebApplication AddBackOfficeTradeCaptureEndpoints(this WebApplication app)
    {
        app.MapPost("/trades", async Task<Results<Created<TradeResponse>, ValidationProblem, ProblemHttpResult>> (
           TradeRequest trade,
           ITradeService tradeService,
           CancellationToken ct) =>
        {
            var errors = TradeRequestValidator.Validate(trade);
            if (errors.Count > 0)
                return TypedResults.ValidationProblem(errors);

            try
            {
                var response = await tradeService.CaptureTradeAsync(trade, ct);
                return TypedResults.Created($"/trades/{response.Id}", response);
            }
            catch (Exception ex)
            {
                // Log the exception here
                return TypedResults.Problem("An error occurred while processing the trade.");
            }
        })
       .WithName("CreateTrade")
       .WithTags("Trades");

        app.MapGet("/reports/trades", async Task<Results<Ok<ReportResponse>, ValidationProblem, BadRequest<string>>> (
           string? from,
           string? to,
           IReportService reportService,
           CancellationToken ct) =>
        {
           
            bool fromValid = DateOnly.TryParse(from, out var fromDate);
            bool toValid = DateOnly.TryParse(to, out var toDate);

            var errors = ReportRequestValidator.Validate(from, fromValid, fromDate, to, toValid, toDate);

            if (errors.Count > 0)
            {
                return TypedResults.ValidationProblem(errors);
            }

            
            try
            {
                var report = await reportService.GetTradeReportAsync(fromDate, toDate, ct);
                return TypedResults.Ok(report);
            }
            catch (Exception)
            {
                return TypedResults.BadRequest("An internal error occurred while generating the report.");
            }
        });

        return app;
    }
}
