using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using ShipIt.PriceQuote.Service;
using ShipIt.PriceQuote.Storage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<PriceQuoteRepositoryOptions>(
    builder.Configuration.GetSection(
        nameof(PriceQuoteRepositoryOptions)
    )
);

builder.Services.AddRateLimiter(_ => _
    .AddFixedWindowLimiter(policyName: "quote-creation-limiter", options =>
    {
        options.PermitLimit = 1;
        options.Window = TimeSpan.FromSeconds(15);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 0;
    }));

builder.Services.AddControllers().AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.Converters.Add(
        new JsonStringEnumConverter());
}).AddMvcOptions(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});
builder.Services.AddScoped<IPriceQuoteRepository, PriceQuoteRepository>();
builder.Services.AddScoped<IPriceQuoteService, PriceQuoteService>();

builder.Services.AddOpenApi(options =>
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
var app = builder.Build();

app.MapControllers();
app.UseRateLimiter();

if (app.Environment.IsDevelopment())
    app.UseExceptionHandler("/error-development");
else
    app.UseExceptionHandler("/error");

app.MapOpenApi();
app.MapScalarApiReference();

app.Run();
