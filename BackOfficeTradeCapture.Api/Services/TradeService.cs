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

        public async Task<TradeResponse> CaptureTradeAsync(TradeRequest trade, CancellationToken ct = default)
        {
            using var transaction = await _db.Database.BeginTransactionAsync(ct);

            try
            {
                var response = await CaptureTradeInternalAsync(trade, ct);
                await transaction.CommitAsync(ct);
                return response;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(ct);
                throw;
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
                BaseCurrency = "EUR"
            };
        }

        private static TradeResponse MapToResponse(TradeEntity entity) =>
           new(
               entity.Id,
               entity.ExternalId,
               entity.Account,
               entity.Symbol,
               entity.Side,
               entity.Quantity,
               entity.Price,
               entity.TradeTime,
               entity.Currency,
               entity.NotionalBase,
               entity.BaseCurrency
           );

        public async Task<BatchTradeResponse> CaptureTradesBatchAsync(BatchTradeRequest request, CancellationToken ct = default)
        {
            var responses = new List<TradeResponse>();

            using var transaction = await _db.Database.BeginTransactionAsync(ct);

            try
            {
                foreach (var trade in request.Trades)
                {
                    var response = await CaptureTradeInternalAsync(trade, ct);
                    responses.Add(response);
                }
                await transaction.CommitAsync(ct);
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }

            return new BatchTradeResponse
            {
                Accepted = responses.Count,
                Trades = responses
            };
        }

        private async Task<TradeResponse> CaptureTradeInternalAsync(TradeRequest trade, CancellationToken ct)
        {
            var existing = await _db.Trades
                .FromSqlInterpolated($"SELECT * FROM Trades WITH (UPDLOCK, ROWLOCK) WHERE ExternalId = {trade.ExternalId}")
                .FirstOrDefaultAsync(ct);

            if (existing != null) return MapToResponse(existing);

            var rate = _currencyService.GetRate(trade.Currency, "EUR");
            var entity = MapToEntity(trade, rate);

            _db.Trades.Add(entity);
            await _db.SaveChangesAsync(ct);

            return MapToResponse(entity);
        }
    }
}
