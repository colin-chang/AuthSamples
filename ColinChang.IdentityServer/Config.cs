// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using System.Collections.Generic;

namespace ColinChang.IdentityServer
{
    public static class Config
    {
        //Identity data
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[] {new IdentityResources.OpenId(),};

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
                new Client
                {
                    ClientId = "ClientCredentialConsoleClient",
                    ClientSecrets = {new Secret("ClientCredentialConsoleClient".Sha256())},
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = {"WeatherApi"}
                }
            };
    }
}