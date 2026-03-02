using ShipIt.Shared;

namespace ShipIt.PriceQuote.Service.Model;

public class PriceQuoteModel
{
    public string Id { get; set; }
    public PriceQuoteModelArguments PriceArguments { get; set; }
    public PriceQuoteModelResult PriceResult { get; set; }
}

public class PriceQuoteModelArguments
{
    public int WidthCm { get; set; }
    public int HeightCm { get; set; }
    public int LengthCm { get; set; }
    public double WeightKg { get; set; }
    public CountryEnum FromCountry { get; set; }
    public CountryEnum ToCountry { get; set; }
}

public class PriceQuoteModelResult
{
    public DateTime CreatedOn { get; set; }
    public DateTime ValidUntil { get; set; }
    public decimal Price { get; set; }
}
