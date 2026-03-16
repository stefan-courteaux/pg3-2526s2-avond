using System;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi;
using ShipIt.PriceQuote.Service;
using ShipIt.PriceQuote.Storage;

namespace ShipIt.PriceQuote.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddShipItControllers(this IServiceCollection services)
    {
        services.AddControllers().AddJsonOptions(opt =>
        {
            opt.JsonSerializerOptions.Converters.Add(
                new JsonStringEnumConverter());
        }).AddMvcOptions(options =>
        {
            options.SuppressAsyncSuffixInActionNames = false;
        });

        return services;
    }

    public static IServiceCollection AddShipitServices(this IServiceCollection services)
    {
        services.AddScoped<IPriceQuoteRepository, PriceQuoteRepository>();
        services.AddScoped<IPriceQuoteService, PriceQuoteService>();

        return services;
    }

    public static IServiceCollection AddShipItRateLimiters(this IServiceCollection services)
    {
        services.AddRateLimiter(_ => _
            .AddFixedWindowLimiter(policyName: "quote-creation-limiter", options =>
            {
                options.PermitLimit = 1;
                options.Window = TimeSpan.FromSeconds(15);
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                options.QueueLimit = 0;
            }));

        return services;
    }

    public static IServiceCollection AddShipItOpenApi(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                // Contact zoals in Demo Microsoft
                document.Info.Contact = new OpenApiContact
                {
                    Name = "ShipIt Support",
                    Email = "support@shipit.com"
                };
                // Description aanpassen
                document.Info.Description = "Via deze api kan je prijzen bereken en uitlezen...";
                return Task.CompletedTask;
            });
        });

        return services;
    }
}
