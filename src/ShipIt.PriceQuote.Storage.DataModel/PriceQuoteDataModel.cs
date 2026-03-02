using System;
using ShipIt.Shared;

namespace ShipIt.PriceQuote.Storage.DataModel;

public class PriceQuoteDataModel
{
    public string id {get;set;} 
    public int widthCm { get; set; }
    public int heightCm { get; set; }
    public int lengthCm { get; set; }
    public double weightKg { get; set; }
    public CountryEnum fromCountry { get; set; }
    public CountryEnum toCountry { get; set; }
    public DateTime createdOn { get; set; }
    public DateTime validUntil { get; set; }
    public decimal price { get; set; }
}
