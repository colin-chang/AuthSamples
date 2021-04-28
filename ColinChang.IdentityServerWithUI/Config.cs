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
                    AccessTokenLifetime = 30

                    // AlwaysIncludeUserClaimsInIdToken = true // 在IdToken中包含所有用户身份声明
                },
                new Client
                {
                    ClientId = "ImplicitJavaScriptClient",
                    RequireClientSecret = false, //不需要客户端认证，所以不需要ClientSecret
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowedScopes =
                    {
                        "WeatherApi",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    },
                    AllowAccessTokensViaBrowser = true,
                    ClientUri = "https://localhost:8000",
                    RedirectUris =
                    {
                        "https://localhost:8000/signin-oidc.html",
                        "https://localhost:8000/silent.html"
                    },
                    PostLogoutRedirectUris = {"https://localhost:8000/signout-oidc.html"},
                    AllowedCorsOrigins = {"https://localhost:8000"},
                    RequireConsent = true, //是否需要用户点击同意
                    AccessTokenLifetime = 5 * 60, //Implicit模式下Token有效时间一般设置较短
                }
            };
    }
}