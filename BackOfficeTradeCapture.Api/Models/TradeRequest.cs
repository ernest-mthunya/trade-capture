using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BackOfficeTradeCapture.Api.Models;
public class TradeRequest : IValidatableObject
{
    private static readonly string[] ValidSides = ["BUY", "SELL"];
    private static readonly string[] ValidCurrencies = ["USD", "EUR", "GBP", "JPY"];

    [JsonPropertyName("external_id")]
    [Required(ErrorMessage = "external_id is required.")]
    [MaxLength(50, ErrorMessage = "external_id must not exceed 50 characters.")]
    public string ExternalId { get; set; } = null!;

    [JsonPropertyName("account")]
    [Required(ErrorMessage = "account is required.")]
    [MaxLength(50, ErrorMessage = "account must not exceed 50 characters.")]
    public string Account { get; set; } = null!;

    [JsonPropertyName("symbol")]
    [Required(ErrorMessage = "symbol is required.")]
    [MaxLength(20, ErrorMessage = "symbol must not exceed 20 characters.")]
    public string Symbol { get; set; } = null!;

    [JsonPropertyName("side")]
    [Required(ErrorMessage = "side is required.")]
    public string Side { get; set; } = null!;

    [JsonPropertyName("quantity")]
    [Range(1, int.MaxValue, ErrorMessage = "quantity must be greater than zero.")]
    public int Quantity { get; set; }

    [JsonPropertyName("price")]
    [Range(0.000001, double.MaxValue, ErrorMessage = "price must be greater than zero.")]
    public decimal Price { get; set; }

    [JsonPropertyName("trade_time")]
    [Required(ErrorMessage = "trade_time is required.")]
    public DateTime TradeTime { get; set; }

    [JsonPropertyName("currency")]
    [Required(ErrorMessage = "currency is required.")]
    [StringLength(3, MinimumLength = 3, ErrorMessage = "currency must be a 3-letter ISO code.")]
    public string Currency { get; set; } = null!;

    [JsonPropertyName("notional_base")]
    public decimal NotionalBase { get; set; } // calculated via SOAP/WCF — not validated on input

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrEmpty(Side) &&
           !ValidSides.Contains(Side.ToUpperInvariant()))
        {
            yield return new ValidationResult(
                $"side must be one of: {string.Join(", ", ValidSides)}.",
                [nameof(Side)]);
        }

        if (!string.IsNullOrEmpty(Currency) &&
            !ValidCurrencies.Contains(Currency.ToUpperInvariant()))
        {
            yield return new ValidationResult(
                $"currency must be one of: {string.Join(", ", ValidCurrencies)}.",
                [nameof(Currency)]);
        }

        if (TradeTime > DateTime.UtcNow)
        {
            yield return new ValidationResult(
                "trade_time cannot be in the future.",
                [nameof(TradeTime)]);
        }
    }
}
