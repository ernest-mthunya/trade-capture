using BackOfficeTradeCapture.Contracts;
using BackOfficeTradeCapture.Wcf;
using CoreWCF;
using CoreWCF.Channels;
using CoreWCF.Configuration;
using CoreWCF.Description;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();

builder.Services.AddSingleton<ServiceMetadataBehavior>(new ServiceMetadataBehavior
{
    HttpGetEnabled = true,
    HttpsGetEnabled = true
});

builder.Services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();

var app = builder.Build();

app.UseServiceModel(serviceBuilder =>
{
    serviceBuilder.AddService<CurrencyRateService>();
    serviceBuilder.AddServiceEndpoint<CurrencyRateService, ICurrencyRateService>(
        new BasicHttpBinding(BasicHttpSecurityMode.Transport),
        "/CurrencyRateService");
});

app.Run();