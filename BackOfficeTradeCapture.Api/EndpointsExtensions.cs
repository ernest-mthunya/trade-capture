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

        return app;
    }
}
