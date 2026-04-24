using System;

namespace ShipIt.Label.Api.Contracts;

public class OpenDataAddressResponseContract
{
    public List<Feature> Features { get; set; }

}

public class Feature{
    public Properties Properties { get; set; }
}

public class Properties
{
    public string Label { get; set; }
    public double Score { get; set; }
}