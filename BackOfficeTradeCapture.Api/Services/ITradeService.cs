using BackOfficeTradeCapture.Api.Entities;
using BackOfficeTradeCapture.Api.Models;

namespace BackOfficeTradeCapture.Api.Services;

public interface ITradeService
{
    Task<TradeEntity> CaptureTradeAsync(TradeRequest request, CancellationToken ct = default);
}

