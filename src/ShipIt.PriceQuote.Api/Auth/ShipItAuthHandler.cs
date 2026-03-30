namespace ShipIt.PriceQuote.Api.Auth;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

public class ShipItAuthHandler() : AuthorizationHandler<ShipItRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ShipItRequirement requirement)
    {
        var isRequiredClaimPresent = context.User.Claims.ToList().Exists(c => c.Value == requirement.Claim);
        var isUserSubPresent = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value is not null;

        // Systeem heeft rechten en er is geen user
        if (isRequiredClaimPresent && !isUserSubPresent)
            context.Succeed(requirement);

        // Systeem heeft rechten, er is een user en die heeft de nodige rol.
        else if (isRequiredClaimPresent && isUserSubPresent && context.User.IsInRole(requirement.Role))
            context.Succeed(requirement);

        // Alle andere gevallen
        else
            context.Fail();

        return Task.CompletedTask;
    }
}