using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ColinChang.IdentityServer.ImplicitJavaScriptClient
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app) =>
            app.UseDefaultFiles().UseStaticFiles();
    }
}