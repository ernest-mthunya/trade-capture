namespace BackOfficeTradeCapture.Api.Models
{
    public record TradeResponse(
      int Id,
      string ExternalId,
      string Account,
      string Symbol,
      string Side,
      decimal Quantity,
      decimal Price,
      DateTimeOffset TradeTime,
      string Currency,
      decimal NotionalBase,
      string BaseCurrency);
}
