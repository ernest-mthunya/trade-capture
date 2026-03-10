using System.Text.Json.Serialization;

namespace BackOfficeTradeCapture.Api.Models;
public class TradeRequest
{
        [JsonPropertyName("external_id")]
        public string ExternalId { get; set; } = null!;

        [JsonPropertyName("account")]
        public string Account { get; set; } = null!;

        [JsonPropertyName("symbol")]
        public string Symbol { get; set; } = null!;

        [JsonPropertyName("side")]
        public string Side { get; set; } = null!;

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("trade_time")]
        public DateTime TradeTime { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; } = null!;

        [JsonPropertyName("notional_base")]
        public decimal NotionalBase { get; set; } // calculated via SOAP/WCF
}
