using System;
using ShipIt.Label.Domain.DTO;

namespace ShipIt.Label.Domain.Model;

internal class Address
{
    public string Name { get; set; }
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string Country { get; set; }

    public Address()
    {

    }

    public Address(LabelCreationDTO labelCreation)
    {
        Name = labelCreation.RecipientName;
        AddressLine1 = labelCreation.RecipientAddressLine1;
        AddressLine2 = labelCreation.RecipientAddressLine2;
        Country = labelCreation.RecipientCountry;
    }
}
