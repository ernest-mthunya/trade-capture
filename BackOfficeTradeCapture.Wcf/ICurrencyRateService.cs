using CoreWCF;

namespace BackOfficeTradeCapture.Wcf;

[ServiceContract]
public interface ICurrencyRateService
{
    [OperationContract]
    decimal GetRate(string fromCurrency, string toCurrency);
}
