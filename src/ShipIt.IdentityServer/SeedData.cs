using System.Security.Claims;
using IdentityModel;
using ShipIt.IdentityServer.Data;
using ShipIt.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;

namespace ShipIt.IdentityServer;

public class SeedData
{
    public static void EnsureSeedData(WebApplication app)
    {
        using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();

            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ShipItUser>>();
            var alice = userMgr.FindByNameAsync("alice").Result;
            if (alice == null)
            {
                alice = new ShipItUser
                {
                    UserName = "alice",
                    Email = "AliceSmith@email.com",
                    EmailConfirmed = true,
                    MijnNotitie = "Dit is Alice"
                };
                var result = userMgr.CreateAsync(alice, "Pass123$").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userMgr.AddClaimsAsync(alice, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Alice Smith"),
                            new Claim(JwtClaimTypes.GivenName, "Alice"),
                            new Claim(JwtClaimTypes.FamilyName, "Smith"),
                            new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                        }).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                Log.Debug("alice created");
            }
            else
            {
                Log.Debug("alice already exists");
            }

            var bob = userMgr.FindByNameAsync("bob").Result;
            if (bob == null)
            {
                bob = new ShipItUser
                {
                    UserName = "bob",
                    Email = "BobSmith@email.com",
                    EmailConfirmed = true,
                    MijnNotitie = "Dit is Bob"
                };
                var result = userMgr.CreateAsync(bob, "Pass123$").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userMgr.AddClaimsAsync(bob, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "Bob Smith"),
                            new Claim(JwtClaimTypes.GivenName, "Bob"),
                            new Claim(JwtClaimTypes.FamilyName, "Smith"),
                            new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                            new Claim("location", "somewhere")
                        }).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                Log.Debug("bob created");
            }
            else
            {
                Log.Debug("bob already exists");
            }
        }

        using (var scope = app.Services
        .GetRequiredService<IServiceScopeFactory>().CreateScope())
        {

            var context = scope.ServiceProvider
                .GetRequiredService<ConfigurationDbContext>();

            Log.Debug("Overwriting db clients with Config.cs");
            context.Clients.RemoveRange(context.Clients);
            foreach (var client in Config.Clients)
                context.Clients.Add(client.ToEntity());
            context.SaveChanges();
            Log.Debug("Clients overwrite done");

            Log.Debug("Adding IdentityResources");
            foreach (var resource in Config.IdentityResources)
                if (!context.IdentityResources.Any(db =>
                        resource.Name == db.Name))
                    context.IdentityResources.Add(resource.ToEntity());
            context.SaveChanges();
            Log.Debug("Adding IdentityResources done");

            Log.Debug("Adding ApiScopes");
            foreach (var resource in Config.ApiScopes)
                if (!context.ApiScopes.Any(db =>
                        resource.Name == db.Name))
                    context.ApiScopes.Add(resource.ToEntity());
            context.SaveChanges();
            Log.Debug("Adding ApiScopes done");
        }
    }
}