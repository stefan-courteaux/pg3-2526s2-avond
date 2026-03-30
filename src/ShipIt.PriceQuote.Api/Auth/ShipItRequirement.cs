using System;
using Microsoft.AspNetCore.Authorization;

namespace ShipIt.PriceQuote.Api.Auth;

public class ShipItRequirement(string claim, string role) : IAuthorizationRequirement
{
    public string Claim { get; } = claim;
    public string Role { get; } = role;
}
