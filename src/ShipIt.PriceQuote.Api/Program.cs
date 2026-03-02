using System.Text.Json.Serialization;
using ShipIt.PriceQuote.Service;
using ShipIt.PriceQuote.Storage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<PriceQuoteRepositoryOptions>(
    builder.Configuration.GetSection(
        nameof(PriceQuoteRepositoryOptions)
    )
);

builder.Services.AddControllers().AddJsonOptions(opt => {
    opt.JsonSerializerOptions.Converters.Add(
        new JsonStringEnumConverter()); 
}).AddMvcOptions(options => {
    options.SuppressAsyncSuffixInActionNames = false; 
});
builder.Services.AddScoped<IPriceQuoteRepository, PriceQuoteRepository>();
builder.Services.AddScoped<IPriceQuoteService, PriceQuoteService>();

var app = builder.Build();

app.MapControllers();

if (app.Environment.IsDevelopment()) 
    app.UseExceptionHandler("/error-development");
else
    app.UseExceptionHandler("/error");

app.Run();
