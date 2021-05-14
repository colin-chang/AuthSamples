// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using System.Collections.Generic;
using IdentityServer4;

namespace ColinChang.IdentityServer
{
    public static class Config
    {
        //Identity data
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };

        //APIs
        public static IEnumerable<ApiScope> ApiScopes =>
            new[]
            {
                new ApiScope("WeatherApi", "天气预报")
            };

        //Clients
        public static IEnumerable<Client> Clients =>
            new[]
            {
                // client credential
                new Client
                {
                    ClientId = "ClientCredentialConsoleClient",
                    ClientSecrets = {new Secret("ClientCredentialConsoleClient".Sha256())},
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = {"WeatherApi"}
                },
                // resource owner password credential
                new Client
                {
                    ClientId = "ResourceOwnerPasswordCredentialClient",
                    ClientSecrets = {new Secret("ResourceOwnerPasswordCredentialClient".Sha256())},
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes =
                    {
                        "WeatherApi",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    }
                },
            };
    }
}