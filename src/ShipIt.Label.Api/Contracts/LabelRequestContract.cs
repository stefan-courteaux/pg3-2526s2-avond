using System;

namespace ShipIt.Label.Api.Contracts;

public record LabelRequestContract
{
    public required string QuoteId { get; set; }
    public required string RecipientName { get; set; }
    public required string RecipientAddressLine1 { get; set; }
    public required string RecipientAddressLine2 { get; set; }
    public required string RecipientCountry { get; set; }
}
