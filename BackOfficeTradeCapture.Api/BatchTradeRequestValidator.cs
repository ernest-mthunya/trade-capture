using BackOfficeTradeCapture.Api.Models;

namespace BackOfficeTradeCapture.Api
{
    public static class BatchTradeRequestValidator
    {
        public static Dictionary<string, string[]> Validate(BatchTradeRequest request)
        {
            var errors = new Dictionary<string, string[]>();

            if (request.Trades is null || request.Trades.Count == 0)
            {
                errors["trades"] = ["Batch must contain at least one trade."];
                return errors;
            }

            for (int i = 0; i < request.Trades.Count; i++)
            {
                var tradeErrors = TradeRequestValidator.Validate(request.Trades[i]);
                foreach (var (key, messages) in tradeErrors)
                    errors[$"trades[{i}].{key}"] = messages;
            }

            return errors;
        }
    }
}
