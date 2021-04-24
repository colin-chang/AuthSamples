// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using System.Collections.Generic;
using IdentityServer4;

namespace ColinChang.IdentityServerWithUI
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new[]
            {
                new ApiScope("WeatherApi", "天气预报")
            };

        public static IEnumerable<Client> Clients =>
            new[]
            {
                new Client
                {
                    ClientId = "AuthorizationCodeMvcClient",
                    ClientSecrets = {new Secret("AuthorizationCodeMvcClient".Sha256())},
                    AllowedGrantTypes = GrantTypes.Code,
                    AllowedScopes =
                    {
                        "WeatherApi",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    },

                    RedirectUris = {"https://localhost:7000/signin-oidc"},
                    FrontChannelLogoutUri = "https://localhost:7000/signout-oidc",
                    PostLogoutRedirectUris = {"https://localhost:7000/signout-callback-oidc"},
                    AllowOfflineAccess = true,
                    AccessTokenLifetime=30

                    // AlwaysIncludeUserClaimsInIdToken = true // 在IdToken中包含所有用户身份声明
                }
            };
    }
}