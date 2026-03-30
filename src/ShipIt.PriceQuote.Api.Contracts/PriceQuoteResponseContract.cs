using ShipIt.Shared;

namespace ShipIt.PriceQuote.Api.Contracts;

public class PriceQuoteResponseContract
{
    public string Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime ValidUntil { get; set; }
    public decimal Price { get; set; }
    public CountryEnum CountryFrom { get; set; }
}
