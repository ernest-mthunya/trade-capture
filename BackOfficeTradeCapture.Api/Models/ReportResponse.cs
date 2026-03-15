using BackOfficeTradeCapture.Api.Models.BackOfficeTradeCapture.Api.Models;
using System.Text.Json.Serialization;

namespace BackOfficeTradeCapture.Api.Models
{
    public record ReportResponse(
        [property: JsonPropertyName("from")] DateOnly From,
        [property: JsonPropertyName("to")] DateOnly To,
        [property: JsonPropertyName("base_ccy")] string BaseCcy,
        [property: JsonPropertyName("rows")] List<ReportRow> Rows);
}