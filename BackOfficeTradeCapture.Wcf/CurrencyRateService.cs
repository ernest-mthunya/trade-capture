

using BackOfficeTradeCapture.Contracts;

namespace BackOfficeTradeCapture.Wcf;

public class CurrencyRateService : ICurrencyRateService
{
    private static readonly Dictionary<string, decimal> Rates = new()
    {
        ["USD"] = 0.92m,   
        ["GBP"] = 1.17m,   
        ["EUR"] = 1.00m, 
        ["JPY"] = 0.0062m
    };
    public decimal GetRate(string fromCurrency, string toCurrency)
    {

        if (fromCurrency == toCurrency) return 1m;

        if (Rates.TryGetValue(fromCurrency.ToUpperInvariant(), out var rate))
            return rate;

        return 1m;
    }
}