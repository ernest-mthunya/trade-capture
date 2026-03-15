using System.Text.Json.Serialization;

namespace BackOfficeTradeCapture.Api.Models
{
    public class ReportRow
    {
        [JsonPropertyName("account")]
        public string Account { get; set; } = default!;

        [JsonPropertyName("symbol")]
        public string Symbol { get; set; } = default!;

        [JsonPropertyName("total_qty")]
        public int TotalQty { get; set; }

        [JsonPropertyName("avg_price")]
        public decimal AvgPrice { get; set; }

        [JsonPropertyName("notional_base")]
        public decimal NotionalBase { get; set; }

        [JsonPropertyName("base_ccy")]
        public string BaseCcy { get; set; } = "EUR";
    }
}
