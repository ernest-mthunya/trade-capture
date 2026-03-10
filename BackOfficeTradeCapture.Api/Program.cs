using BackOfficeTradeCapture.Api;
using BackOfficeTradeCapture.Api.Models;
using BackOfficeTradeCapture.Contracts;
using System.ServiceModel;

var builder = WebApplication.CreateBuilder(args);

// Now you can simply use the type from your shared library
builder.Services.AddScoped<ICurrencyRateService>(sp =>
{
    var binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
    var endpoint = new EndpointAddress("https://localhost:62411/CurrencyRateService");
    return new ChannelFactory<ICurrencyRateService>(binding, endpoint).CreateChannel();
});



// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.AddBackOfficeTradeCaptureEndpoints();

app.Run();
