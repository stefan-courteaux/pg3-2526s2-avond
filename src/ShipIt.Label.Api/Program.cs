using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using QuestPDF.Infrastructure;
using ShipIt.Label.Api.Contracts;
using ShipIt.Label.Domain;
using ShipIt.Label.Domain.DTO;
using ShipIt.Label.Persistence;
using ShipIt.PriceQuote.Api.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<LabelRepositoryOptions>(
    builder.Configuration.GetSection(nameof(LabelRepositoryOptions))
);

builder.Services.AddHttpClient();
builder.Services.AddScoped<IDomainController, DomainController>();
builder.Services.AddScoped<ILabelRepository, LabelRepository>();

builder.Services.AddRateLimiter(_ => _
    .AddFixedWindowLimiter(policyName: "label-read-limiter", options =>
    {
        options.PermitLimit = 1;
        options.Window = TimeSpan.FromSeconds(15);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 0;
    }));

QuestPDF.Settings.License = LicenseType.Community;

var app = builder.Build();

app.UseRateLimiter();

app.MapGet("/", () => "Hello World!");

app.MapPost("labels", async Task<CreatedAtRoute<string>> (
    IHttpClientFactory httpClientFactory,
    [FromBody] LabelRequestContract requestContract,
    IDomainController domainController,
    IConfiguration config) =>
{
    // get price op basis van quoteId
    var httpClient = httpClientFactory.CreateClient();
    httpClient.BaseAddress = new Uri(config["QuoteApiBaseUrl"]);

    var priceQuote = await httpClient.GetFromJsonAsync<PriceQuoteResponseContract>($"pricequotes/{requestContract.QuoteId}");

    // could validate
    // - is price quote still valid?
    // - does recipient country match price quote country?

    // label maken
    var dto = new LabelCreationDTO
    {
        RecipientAddressLine1 = requestContract.RecipientAddressLine1,
        RecipientAddressLine2 = requestContract.RecipientAddressLine2,
        RecipientName = requestContract.RecipientName,
        RecipientCountry = requestContract.RecipientCountry,
        ShipmentPrice = (double)priceQuote.Price
    };
    var tn = await domainController.CreateLabelAsync(dto);

    // return id (location if possible)
    return TypedResults.CreatedAtRoute(tn, "GetLabel", new { trackingNumber = tn });
});

app.MapGet("labels/{trackingnumber}",
async Task<Results<Ok<string>, BadRequest, FileContentHttpResult>> (
    [FromRoute] string trackingNumber,
    [FromHeader(Name = "Accept")] string acceptValue,
    IDomainController domain) =>
{
    if (acceptValue == "text/json")
    {
        // Dummy implementatie
        return TypedResults.Ok(trackingNumber);
    }
    else if (acceptValue == "application/pdf")
    {
        var pdf = await domain.GetLabelAsync(trackingNumber);
        return TypedResults.Bytes(
            pdf,
            "application/pdf",
            $"DitIsDeFileNaam{trackingNumber}.pdf");
    }

    return TypedResults.BadRequest();

}).WithName("GetLabel")
.RequireRateLimiting("label-read-limiter");

app.Run();
