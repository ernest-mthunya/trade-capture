using BackOfficeTradeCapture.Api.Data;
using BackOfficeTradeCapture.Api.Services;
using BackOfficeTradeCapture.Contracts;
using Microsoft.EntityFrameworkCore;
using System.ServiceModel;

namespace BackOfficeTradeCapture.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BackOfficeDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString(ApplicationConstants.ConnectionStrings.DefaultConnection))
                   .LogTo(Console.WriteLine, LogLevel.Information));

        return services;
    }

    public static IServiceCollection AddTradeServices(this IServiceCollection services)
    {
        services.AddScoped<ITradeService, TradeService>();
        services.AddScoped<IReportService, ReportService>();

        return services;
    }

    public static IServiceCollection AddCurrencyRateService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICurrencyRateService>(sp =>
        {
            var url = configuration[ApplicationConstants.CurrencyService.Url]!;
            var binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            var endpoint = new EndpointAddress(url);
            return new ChannelFactory<ICurrencyRateService>(binding, endpoint).CreateChannel();
        });

        return services;
    }
}