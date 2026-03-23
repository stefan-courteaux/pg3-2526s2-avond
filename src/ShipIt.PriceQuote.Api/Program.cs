using Scalar.AspNetCore;
using ShipIt.PriceQuote.Api;
using ShipIt.PriceQuote.Storage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<PriceQuoteRepositoryOptions>(
    builder.Configuration.GetSection(
        nameof(PriceQuoteRepositoryOptions)
    )
);

builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.Authority = "https://localhost:5001";
        options.TokenValidationParameters.ValidateAudience = false;
    });

builder.Services.AddAuthorization();

builder.Services.AddShipItRateLimiters();
builder.Services.AddShipItControllers();
builder.Services.AddShipitServices();
builder.Services.AddOpenApi();

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
