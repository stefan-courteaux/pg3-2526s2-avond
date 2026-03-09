using System;

namespace ShipIt.Label.Domain.DTO;

public class LabelCreationDTO
{
    public string RecipientName { get; set; }
    public string RecipientAddressLine1 { get; set; }
    public string RecipientAddressLine2 { get; set; }
    public string RecipientCountry { get; set; }
    public double ShipmentPrice { get; set; }
}
