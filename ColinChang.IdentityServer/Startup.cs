// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ColinChang.IdentityServer
{
    public class Startup
    {
        private readonly IHostEnvironment _env;
        public Startup(IHostEnvironment evn) => _env = evn;

        public void ConfigureServices(IServiceCollection services)
        {
            var builder = services
                .AddIdentityServer(options => options.EmitStaticAudienceClaim = true)
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryClients(Config.Clients)
                .AddTestUsers(TestUsers.Users);

            if (_env.IsProduction())
                builder.AddSigningCredential(
                    new X509Certificate2(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "is4.pfx"),
                        "5C6CE27CBA3DD15B4EFBE5A7EC679CBBE79D14F5"));
            else
                builder.AddDeveloperSigningCredential();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
        {
            if (environment.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseIdentityServer();
        }
    }
}