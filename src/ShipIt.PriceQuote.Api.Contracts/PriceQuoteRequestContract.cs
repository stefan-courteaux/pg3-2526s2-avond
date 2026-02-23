using System;

namespace ShipIt.PriceQuote.Api.Contracts;

public class PriceQuoteRequestContract
{
    public int WidthCm { get; set; }
    public int HeightCm { get; set; }
    public int LengthCm { get; set; }
    public double WeightKg { get; set; }
    public CountryEnum FromCountry { get; set; }
    public CountryEnum ToCountry { get; set; }
}
