

using BackOfficeTradeCapture.Contracts;

namespace BackOfficeTradeCapture.Wcf;

public class CurrencyRateService : ICurrencyRateService
{
    public decimal GetRate(string fromCurrency, string toCurrency)
    {
        // Stub: always return 1.09 for simplicity
        return 1.09M;
    }
}