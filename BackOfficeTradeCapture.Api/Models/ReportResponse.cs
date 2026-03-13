using System.Text.Json.Serialization;

namespace BackOfficeTradeCapture.Api.Models
{
    public class ReportResponse
    {
        [JsonPropertyName("from")]
        public DateOnly From { get; set; }

        [JsonPropertyName("to")]
        public DateOnly To { get; set; }

        [JsonPropertyName("base_ccy")]
        public string BaseCcy { get; set; } = "EUR";

        [JsonPropertyName("rows")]
        public List<ReportRow> Rows { get; set; } = [];
    }
}
