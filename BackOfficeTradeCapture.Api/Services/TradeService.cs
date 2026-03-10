using BackOfficeTradeCapture.Api.Data;
using BackOfficeTradeCapture.Api.Entities;
using BackOfficeTradeCapture.Api.Models;
using BackOfficeTradeCapture.Contracts;
using Microsoft.EntityFrameworkCore;

namespace BackOfficeTradeCapture.Api.Services
{
    public class TradeService : ITradeService
    {
        private readonly BackOfficeDbContext _db;
        private readonly ICurrencyRateService _currencyService;

        public TradeService(BackOfficeDbContext db, ICurrencyRateService currencyService)
        {
            _db = db;
            _currencyService = currencyService;
        }

        public async Task<TradeEntity> CaptureTradeAsync(TradeRequest trade, CancellationToken ct = default)
        {
            // Check for existence FIRST
            var existing = await _db.Trades
                .FirstOrDefaultAsync(t => t.ExternalId == trade.ExternalId, ct);

            if (existing != null)
            {
                return existing; // Already processed, return the existing one
            }

            var rate = _currencyService.GetRate(trade.Currency, "EUR");

            // Only add if it does not exist
            var entity = MapToEntity(trade, rate);
            _db.Trades.Add(entity);

            try
            {
                await _db.SaveChangesAsync(ct);
                return entity;
            }
            catch (DbUpdateException)
            {
                // Double-check if someone inserted it in the microsecond between Check and Save
                return await _db.Trades.FirstAsync(t => t.ExternalId == trade.ExternalId, ct);
            }

        }

        private static TradeEntity MapToEntity(TradeRequest request, decimal rate)
        {
            return new TradeEntity
            {
                ExternalId = request.ExternalId,
                Account = request.Account,
                Symbol = request.Symbol,
                Side = request.Side,
                Quantity = request.Quantity,
                Price = request.Price,
                TradeTime = request.TradeTime,
                Currency = request.Currency,
                NotionalBase = request.Quantity * request.Price * rate,
                BaseCurrency = "EUR" // Fixed base currency per your requirements
            };
        }

    }
}
