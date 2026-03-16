using System.Text.Json.Serialization;

namespace ShipIt.Shared;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CountryEnum
{
    BE = 0,
    NL = 1,
    FR = 2,
    LU = 3
}
