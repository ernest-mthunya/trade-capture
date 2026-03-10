using BackOfficeTradeCapture.Api;
using BackOfficeTradeCapture.Api.Models;
using BackOfficeTradeCapture.Wcf;
using CoreWCF;
using CoreWCF.Channels;
using CoreWCF.Configuration;

var builder = WebApplication.CreateBuilder(args);



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
