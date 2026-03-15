using System.Text.Json.Serialization;

namespace BackOfficeTradeCapture.Api.Models
{
    using System.Text.Json.Serialization;

    namespace BackOfficeTradeCapture.Api.Models
    {
        public record ReportRow(
            [property: JsonPropertyName("account")] string Account,
            [property: JsonPropertyName("symbol")] string Symbol,
            [property: JsonPropertyName("total_qty")] int TotalQty,
            [property: JsonPropertyName("avg_price")] decimal AvgPrice,
            [property: JsonPropertyName("notional_base")] decimal NotionalBase,
            [property: JsonPropertyName("base_ccy")] string BaseCcy);
    }
}
