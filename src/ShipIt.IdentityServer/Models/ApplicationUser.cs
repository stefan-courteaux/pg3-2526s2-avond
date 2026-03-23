// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Identity;

namespace ShipIt.IdentityServer.Models;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ShipItUser : IdentityUser
{
    public string? MijnNotitie { get; set; }
}
