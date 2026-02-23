namespace ShipIt.PriceQuote.Api.Contracts;

public class PriceQuoteResponseContract
{
    public DateTime CreatedOn { get; set; }
    public DateTime ValidUntil { get; set; }
    public decimal Price { get; set; }
}
