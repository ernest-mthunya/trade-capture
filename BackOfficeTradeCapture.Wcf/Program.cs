using BackOfficeTradeCapture.Wcf;
using CoreWCF;
using CoreWCF.Channels;
using CoreWCF.Configuration;
using CoreWCF.Description;

var builder = WebApplication.CreateBuilder(args);

// 1. Register CoreWCF services
builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();

// 2. Configure Metadata Behavior via DI
// This ensures the middleware picks up these settings globally
builder.Services.AddSingleton<ServiceMetadataBehavior>(new ServiceMetadataBehavior
{
    HttpGetEnabled = true,
    HttpsGetEnabled = true
});

// 3. Fix for WSDL URLs
// This helps CoreWCF generate the correct 'location' attributes in the WSDL
// by using the actual request headers (localhost:62411)
builder.Services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();

var app = builder.Build();

// Configure the CoreWCF Service Pipeline
app.UseServiceModel(serviceBuilder =>
{
    // Register the service implementation
    serviceBuilder.AddService<CurrencyRateService>();

    // Add the BasicHttpBinding endpoint
    // The WSDL will be available at /CurrencyRateService?wsdl
    serviceBuilder.AddServiceEndpoint<CurrencyRateService, ICurrencyRateService>(
        new BasicHttpBinding(BasicHttpSecurityMode.Transport), // Use Transport for HTTPS
        "/CurrencyRateService");
});

app.Run();