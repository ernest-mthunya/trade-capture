namespace BackOfficeTradeCapture.Api.Models
{
    public record BatchTradeRequest
    {
        public List<TradeRequest> Trades { get; init; } = [];
    }
}
