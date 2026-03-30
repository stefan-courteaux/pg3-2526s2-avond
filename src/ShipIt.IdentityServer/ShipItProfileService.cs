using System;
using System.Security.Claims;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using ShipIt.IdentityServer.Models;

namespace ShipIt.IdentityServer;

public class ShipItProfileService(UserManager<ShipItUser> users) : IProfileService
{
    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var user = await users.GetUserAsync(context.Subject)
            ?? throw new Exception("Cannot map roles for (null) user!");

        var roles = await users.GetRolesAsync(user);

        var claims = roles.Select(role => new Claim("role", role));
        context.IssuedClaims.AddRange(claims);
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        var existing = await users
            .FindByIdAsync(context.Subject.GetSubjectId());
        context.IsActive = existing?.LockoutEnd is null;
    }
}
