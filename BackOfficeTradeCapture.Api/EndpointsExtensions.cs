using BackOfficeTradeCapture.Api.Models;
using Microsoft.AspNetCore.Builder;

namespace BackOfficeTradeCapture.Api;
public static class EndpointsExtensions
{
    public static WebApplication AddBackOfficeTradeCaptureEndpoints(this WebApplication app)
    {
        app.MapPost("/trades", (TradeRequest trade) =>
        {
            //if (trade is null)
            //    return Results.BadRequest("Request body is required.");

            //if (string.IsNullOrWhiteSpace(trade.ExternalId))
            //    return Results.BadRequest("external_id is required.");

            //// In a real app you'd persist the trade here. For now return Created with the submitted payload.
            //var location = $"/trades/{trade.ExternalId}";
            //return Results.Created(location, trade);
        })
        .WithName("CreateTrade")
        .WithTags("Trades");

        return app;
    }
}
