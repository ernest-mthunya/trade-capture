using System.ServiceModel;

namespace BackOfficeTradeCapture.Contracts;

[ServiceContract]
public interface ICurrencyRateService
{
    [OperationContract]
    decimal GetRate(string fromCurrency, string toCurrency);
}
