namespace BackOfficeTradeCapture.Api.Models
{
    public record BatchTradeResponse
    {
        public int Accepted { get; init; }
        public List<TradeResponse> Trades { get; init; } = [];
    }
}
