using System.Threading.RateLimiting;
using Duende.IdentityModel.Client;
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

    //TODO move to config
    var isUrl = "https://localhost:5001";
    var disco = await httpClient.GetDiscoveryDocumentAsync(isUrl);

    // Get token
    var tokenResponse = await httpClient
        .RequestClientCredentialsTokenAsync(
            new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "label-client",
                ClientSecret = "eenGrootLabelGeheim",
                Scope = "shipit.pricequotes.api.read"
            });

    // Set token (before calling pricequote api)
    httpClient.SetBearerToken(tokenResponse.AccessToken ?? throw new Exception("Could not obtain token."));

    // Call pricequote api
    var fullQuoteEndpoint = new Uri($"{config["QuoteApiBaseUrl"] ?? throw new Exception("QuoteApiBaseUrl is unknown")}/pricequotes/{requestContract.QuoteId}");
    var priceQuote = await httpClient.GetFromJsonAsync<PriceQuoteResponseContract>(fullQuoteEndpoint) ?? throw new Exception("PriceQuote response is null. Cannot continue.");

    // could validate response
    // - is price quote still valid?
    // - does recipient country match price quote country? etc...

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
