using BackOfficeTradeCapture.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace BackOfficeTradeCapture.Api
{
    public static class TradeRequestValidator
    {
        public static Dictionary<string, string[]> Validate(TradeRequest request)
        {
            var context = new ValidationContext(request);
            var results = new List<ValidationResult>();

            // Validates both DataAnnotations AND IValidatableObject.Validate()
            Validator.TryValidateObject(request, context, results, validateAllProperties: true);

            return results
                .GroupBy(r => r.MemberNames.FirstOrDefault() ?? "general")
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(r => r.ErrorMessage ?? "Invalid value.").ToArray()
                );
        }
    }
}
