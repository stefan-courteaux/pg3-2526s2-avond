using Microsoft.AspNetCore.Authorization;
using Scalar.AspNetCore;
using ShipIt.PriceQuote.Api;
using ShipIt.PriceQuote.Api.Auth;
using ShipIt.PriceQuote.Storage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<PriceQuoteRepositoryOptions>(
    builder.Configuration.GetSection(
        nameof(PriceQuoteRepositoryOptions)
    )
);

builder.Services.AddSingleton<IAuthorizationHandler, ShipItAuthHandler>();

builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.Authority = "https://localhost:5001";
        options.TokenValidationParameters.ValidateAudience = false;
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("QuoteReadPolicy", policy =>
        policy.Requirements.Add(
            new ShipItRequirement("shipit.pricequotes.api.read", "Quoter")))
    .AddPolicy("QuoteWritePolicy", policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim("scope",
                "shipit.pricequotes.api.write");
        });

builder.Services.AddShipItRateLimiters();
builder.Services.AddShipItControllers();
builder.Services.AddShipitServices();
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});

var app = builder.Build();

app.MapControllers();
app.UseRateLimiter();

app.UseRouting();
app.UseCors();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
    app.UseExceptionHandler("/error-development");
else
    app.UseExceptionHandler("/error");

app.MapOpenApi();
app.MapScalarApiReference();

app.Run();
