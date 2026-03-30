using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace ShipIt.IdentityServer;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("shipit.pricequotes.api.read"),
            new ApiScope("shipit.pricequotes.api.write")
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            // machine to machine client credentials flow client
            new Client
            {
                ClientId = "postman-client",
                ClientName = "Postman Client Credentials Client",

                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("eenGrootGeheim".Sha256()) },

                AllowedScopes = { "shipit.pricequotes.api.read", "shipit.pricequotes.api.write" }
            },
            new Client
            {
                ClientId = "label-client",
                ClientName = "Label Client Credentials Client",

                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("eenGrootLabelGeheim".Sha256()) },

                AllowedScopes = { "shipit.pricequotes.api.read" }
            },
            // Interactive OIDC
            new Client {
                ClientId = "react-spa-client",
                ClientSecrets = {new Secret("react-secret".Sha256())},
                AllowedGrantTypes = GrantTypes.Code,
                AllowedScopes = {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "shipit.pricequotes.api.read"
                },
                RedirectUris = { "http://localhost:5173/" },
                PostLogoutRedirectUris = { "http://localhost:5173/" },
                AllowedCorsOrigins = ["http://localhost:5173"],
            }
        };
}
